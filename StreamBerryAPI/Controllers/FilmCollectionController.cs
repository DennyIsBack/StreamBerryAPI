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
        public async Task<ActionResult<List<Film>>> ConsultListFilms([FromQuery] int PageNumber = 0, [FromQuery] int PageSize = 20)
        {
            try
            {
                var Films = await context.ListFilmAsync(PageNumber, PageSize);

                return Ok(Films);
            }
            catch (Exception ex)
            {
                return BadRequest("ocorreu uma exceção:" + ex.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<Film>>> ConsultFilmByTitle([FromQuery] string title, [FromQuery] int PageNumber = 0, [FromQuery] int PageSize = 20)
        {
            if (string.IsNullOrEmpty(title))
                return NotFound("Titulo do filme não foi especificado");

            var Films = await context.ConsultFilmByTitleAsync(title, PageNumber, PageSize);


            return Ok(Films);
        }

        [HttpGet]
        public async Task<ActionResult<Film>> ConsultFilmByYear([FromQuery] int PageNumber = 0, [FromQuery] int PageSize = 20, [FromQuery] int? Year = null)
        {
            //caso não seja informado o ano, sera utilizado o ano base como filtro de consulta dos 20 primeiros
            if (!Year.HasValue)
                Year = DateTime.Now.Year;

            var Films = await context.ConsultFilmByYearAsync((int)Year, PageNumber, PageSize);

            return Ok(Films);
        }

        [HttpGet]
        public async Task<ActionResult<List<Film>>> ConsultFilmByRating([FromQuery] int Rating, [FromQuery] int PageNumber = 0, [FromQuery] int PageSize = 20)
        {
            if (Rating > 0 && Rating < 5)
                return await context.ConsultFilmByRatingAsync(Rating, PageNumber, PageSize);
            else
                return BadRequest("A avaliação deve ser entre 1 e 5");
        }

        [HttpGet]
        public async Task<ActionResult<FilmVoteAverageByGenre>> VoteAverageByGenreYear([FromQuery] string Genre, [FromQuery] int Year, [FromQuery] int PageNumber = 0, [FromQuery] int PageSize = 20)
        {
            if (string.IsNullOrEmpty(Genre) || Year == 0)
                return BadRequest("Os parametros Genre e Year são obrigatorios");

            var ret = await context.VoteAverageByGenreYearAsync(Genre, Year, PageNumber, PageSize);

            return Ok();
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

                if (film.Month < 1 || film.Month > 12)
                    return BadRequest("O mês informado não é valido.");

                if (film.Year < 1950 || film.Year > DateTime.Now.Year || film.Year.ToString().Length < 3)
                    return BadRequest("O ano informado não é valido.");

                film.CalculateAverage(); //calcular media do filme antes de gravar no banco

                var Films = await context.UpdateFilmAsync(film);


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

                if(film.Month < 1 || film.Month > 12)
                    return BadRequest("O mês informado não é valido.");

                if (film.Year < 1950 || film.Year > DateTime.Now.Year || film.Year.ToString().Length < 3)
                    return BadRequest("O ano informado não é valido.");

                var Filter = new Film()
                {
                    Title = film.Title,
                    Reviews = film.Reviews,
                    Year = film.Year,
                    Genre = film.Genre,
                    Streaming = film.Streaming,
                    Month = film.Month
                };

                Filter.CalculateAverage(); //calcular media do filme antes de gravar no banco

                var ret = await context.CreateFilmAsync(Filter);

                return Ok(ret);

            }
            catch (Exception ex)
            {
                return BadRequest("ocorreu uma exceção:" + ex.Message);
            }
        }

        [HttpDelete]
        public async Task<ObjectResult> DeleteFilm([FromQuery] int id)
        {
            var ret = await context.DeleteAsync(id);

            if (ret == true)
                return Ok("Deletado com sucesso");
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
