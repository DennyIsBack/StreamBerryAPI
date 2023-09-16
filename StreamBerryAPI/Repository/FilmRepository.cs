using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using StreamBerryAPI.Data;
using StreamBerryAPI.Models;
using StreamBerryAPI.Repository.Interface;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace StreamBerryAPI.Repository
{
    public class FilmRepository : IFilmRepository
    {
        //testar se da erro no required
        private readonly FilmDBContext _dbContext;

        public FilmRepository(FilmDBContext dBContext)
        {
            _dbContext = dBContext;
        }

        public async Task<RetPaged<Film>> ListFilm(int pageNumber = 0, int PageSize = 20)
        {
            var ret = new RetPaged<Film>();

            ret.TotalData = await _dbContext.Film.CountAsync();
            ret.TotalPage = Math.Abs(ret.TotalData / PageSize);

            var data = await _dbContext.Film.AsNoTracking()
                                            .GroupBy(x => x.GenreId)
                                            .OrderBy(g => g.Key)
                                            .Select(group => group.OrderBy(film => film.Title)) // Ordena os filmes dentro de cada genero por título
                                            .Skip(pageNumber * PageSize)
                                            .Take(PageSize)
                                            .ToListAsync();

            ret.Data = data.SelectMany(group => group).ToList();

            return ret;
        }
        public async Task<List<Film>> ConsultFilmByTitle(string Title, int pageNumber = 0, int PageSize = 20)
        {
            return await _dbContext.Film.Where(x => x.Title.Contains(Title)).OrderBy(x => x.Title).Skip(pageNumber * PageSize).Take(PageSize).ToListAsync();
        }

        public async Task<GenericModelByYear<Film>> ConsultFilmByYear(int Year, int pageNumber = 0, int PageSize = 20)
        {
            var ret = new GenericModelByYear<Film>();           

            var query = _dbContext.Film.AsQueryable();

            TotalByYear filter = new TotalByYear();

            var yearCounts = await _dbContext.Film
                                             .GroupBy(x => x.Year)
                                             .Select(g => new
                                             {
                                                 Year = g.Key, // Obtém o ano
                                                 Total = g.Count() // Obtém a contagem de filmes para o ano
                                             })
                                             .ToListAsync();

            foreach (var yearCount in yearCounts)
            {
                var item = new TotalByYear
                {
                    Year = yearCount.Year,
                    TotalYear = yearCount.Total
                };

                ret.TotalYear.Add(item);
            }


            query = query.Where(x => x.Year.Equals(Year)).OrderBy(x => x.Title).Skip(pageNumber * PageSize).Take(PageSize);

            ret.FilterByYear = Year;

            ret.Data = await query.ToListAsync();

            return ret;
        }

        public async Task<List<Film>> ConsultFilmByRating(int Average, int pageNumber = 0, int PageSize = 20)
        {
            return await _dbContext.Film.Where(x => x.VoteAverage.Equals(Average)).OrderBy(x => x.Title).Skip(pageNumber * PageSize).Take(PageSize).ToListAsync();
        }

        public async Task<Film> CreateFilm(Film film)
        {
            await _dbContext.Film.AddAsync(film);
            await _dbContext.SaveChangesAsync();
            return film;
        }

        public async Task<Film> UpdateFilm(CreateFilm film)
        {
            Film? ConsultFilm = await _dbContext.Film.FirstOrDefaultAsync(x => x.Id == film.Id);

            if (ConsultFilm == null)
            {
                throw new Exception($"Não foi encontrado nenhum filme com o ID: {film.Id}");
            }

            ConsultFilm.Title = film.Title;
            ConsultFilm.Reviews = film.Reviews;
            ConsultFilm.GenreId = film.GenreId;
            ConsultFilm.Month = film.Month;
            ConsultFilm.Year = film.Year;
            ConsultFilm.StreamingId = film.StreamingId;

            _dbContext.Film.Update(ConsultFilm);
            await _dbContext.SaveChangesAsync();

            return ConsultFilm;
        }
        public async Task<bool> Delete([Required] int id)
        {
            Film? ConsultFilm = await _dbContext.Film.FirstOrDefaultAsync(x => x.Id == id);

            if (ConsultFilm == null)
                return false;

            _dbContext.Film.Remove(ConsultFilm);
            await _dbContext.SaveChangesAsync();
            return true;
        }

    }
}