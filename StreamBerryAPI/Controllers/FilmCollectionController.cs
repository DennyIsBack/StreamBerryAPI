using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StreamBerryAPI.Data;
using StreamBerryAPI.Models;
using StreamBerryAPI.Repository.Interface;

namespace StreamBerryAPI.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class FilmCollectionController : ControllerBase
    {
        private readonly IFilmRepository context;

        public FilmCollectionController(IFilmRepository dbContext)
        {
            context = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<List<Film>>> ConsultListFilms([FromBody] int PageNumber, [FromBody] int PageSize)
        {
            try
            {
                var Films = await context.ListFilm(PageNumber, PageSize);

                return Ok(Films);
            }
            catch (Exception ex)
            {
                return BadRequest("ocorreu uma exceção:" + ex.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<Film>>> ConsultFilmByTitle([FromBody] string title, [FromBody] int PageNumber, [FromBody] int PageSize)
        {
            if (string.IsNullOrEmpty(title))
                return NotFound("Titulo do filme não foi especificado");

            var Films = await context.ConsultFilmByTitle(title, PageNumber, PageSize);


            return Ok(Films);
        }

        [HttpGet]
        public async Task<ActionResult<List<Film>>> ConsultFilmByYear([FromBody] int PageNumber, [FromBody] int PageSize, [FromBody] int? Year = null)
        {
            //caso não seja informado o ano, sera utilizado o ano base como filtro de consulta dos 20 primeiros
            if (!Year.HasValue)
                Year = DateTime.Now.Year;

            var Films = await context.ConsultFilmByYear((int)Year, PageNumber, PageSize);

            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<List<Film>>> ConsultFilmByRating([FromBody] int Rating, [FromBody] int PageNumber, [FromBody] int PageSize)
        {
            if (Rating > 0 && Rating < 5)
                return await context.ConsultFilmByRating(Rating, PageNumber, PageSize);
            else
                return BadRequest("A avaliação deve ser entre 1 e 5");
        }

        [HttpPut]
        public async Task<ActionResult<Film>> UpdateFilm([FromBody] CreateFilm film)
        {
            if (!ModelState.IsValid)
            {
                var errorMessages = ModelState
                    .Where(m => m.Value.Errors.Any())
                    .Select(m => new { Field = m.Key, Errors = m.Value.Errors.Select(e => e.ErrorMessage).ToList() })
                    .ToList();

                return BadRequest(errorMessages);
            }        

            try
            {
                if (film.Reviews != null && film.Reviews.Any())
                    foreach (var review in film.Reviews)
                    {
                        if (!ValidReview(review))
                            return BadRequest("Não é possivel salvar uma avaliação com comentario sem uma classificação selecionada.");
                    }

                film.CalculateAverage(); //calcular media do filme antes de gravar no banco

                var Films = await context.UpdateFilm(film);


                return Ok(Films);
            }
            catch (Exception ex)
            {
                return BadRequest("ocorreu uma exceção:" + ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<Film>> CreateFilm([FromBody] CreateFilm film)
        {
            if (!ModelState.IsValid)
            {
                var errorMessages = ModelState
                    .Where(m => m.Value.Errors.Any())
                    .Select(m => new { Field = m.Key, Errors = m.Value.Errors.Select(e => e.ErrorMessage).ToList() })
                    .ToList();

                return BadRequest(errorMessages);
            }
            try
            {
                if (film.Reviews != null && film.Reviews.Any())
                    foreach (var review in film.Reviews)
                    {
                        if (!ValidReview(review))
                            return BadRequest("Não é possivel salvar uma avaliação com comentario sem uma classificação selecionada.");
                    }

                var Filter = new Film()
                {
                    Title = film.Title,
                    Reviews = film.Reviews,
                    Year = film.Year,
                    GenreId = film.GenreId,
                    StreamingId = film.StreamingId,
                    Month = film.Month
                };

                Filter.CalculateAverage(); //calcular media do filme antes de gravar no banco

                var ret = await context.CreateFilm(Filter);

                return Ok(ret);

            }
            catch (Exception ex)
            {
                return BadRequest("ocorreu uma exceção:" + ex.Message);
            }
        }

        [HttpDelete]
        public async Task<ActionResult<bool>> DeleteFilm([FromBody] int id)
        {
            var ret = await context.Delete(id);

            if (ret == true)
                return Ok(ret);
            else
                return BadRequest($"Não foi encontrado nenhum filme com o ID: {id}");
        }

        private bool ValidReview(Review review)
        {
            if (!string.IsNullOrEmpty(review.Comments))
            {
                if (review.Rating == 0)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
