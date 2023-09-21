using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using StreamBerryAPI.Data;
using StreamBerryAPI.Models;
using StreamBerryAPI.Repository.Interface;
using System;
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

        public async Task<RetPaged<Film>> ListFilmAsync(int pageNumber, int PageSize)
        {
            var ret = new RetPaged<Film>();

            ret.TotalData = await _dbContext.Film.CountAsync();
            ret.TotalPage = (int)Math.Ceiling((double)ret.TotalData / PageSize);

            var data = await _dbContext.Film.AsNoTracking()
                                            .Include(x => x.Genre)
                                            .Include(x => x.Streaming)
                                            .Include(x => x.Reviews)
                                            .OrderBy(g => g.Title)
                                            .Skip(pageNumber * PageSize)
                                            .Take(PageSize)
                                            .ToListAsync();

            ret.Data = data.ToList();

            return ret;
        }
        public async Task<List<Film>> ConsultFilmByTitleAsync(string Title, int pageNumber = 0, int PageSize = 20)
        {
            return await _dbContext.Film.Where(x => x.Title.Contains(Title))
                .Include(x => x.Genre)
                .Include(x => x.Streaming)
                .Include(x => x.Reviews)
                .OrderBy(g => g.Title)
                .OrderBy(x => x.Title)
                .Skip(pageNumber * PageSize)
                .Take(PageSize)
                .ToListAsync();
        }

        public async Task<GenericModelByYear<Film>> ConsultFilmByYearAsync(int Year, int pageNumber = 0, int PageSize = 20)
        {
            var ret = new GenericModelByYear<Film>();

            var query = _dbContext.Film.AsQueryable();

            TotalByYear filter = new TotalByYear();

            var yearCounts = await _dbContext.Film
                                             .GroupBy(x => x.Year)
                                             .Select(g => new
                                             {
                                                 Year = g.Key, // Obtém o ano
                                                 Total = g.Count() // Obtém a contagem de filmes do ano
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

            ret.Data = await query.Include(x => x.Genre).Include(x => x.Streaming).Include(x => x.Reviews).ToListAsync();

            return ret;
        }

        public async Task<FilmVoteAverageByGenre> VoteAverageByGenreYearAsync(string Genre, int Year, int pageNumber = 0, int PageSize = 20)
        {
            var ret = new FilmVoteAverageByGenre();

            var query = _dbContext.Film.AsQueryable();

            // Filtrar filmes pelo gênero
            query = query.Where(f => f.Genre.Any(g => g.Description == Genre));


            // Filtrar filmes pelo ano
            query = query.Where(f => f.Year == Year);

            ret.Genre = Genre;
            ret.Year = Year;
            ret.TotalVoteAverage = (int)Math.Round(query.Average(g => g.VoteAverage));
            ret.Data = await query.OrderBy(x => x.Title).Include(x => x.Genre).Include(x => x.Streaming).Include(x => x.Reviews).Skip(pageNumber * PageSize).Take(PageSize).ToListAsync();

            return ret;
        }
        public async Task<List<AverageByGenreYear>> AllVoteAveragebyGenreYear()
        {
            var query = _dbContext.Film
                        .Include(f => f.Genre)
                        .AsNoTracking();

            var Data = await query
                                .SelectMany(f => f.Genre.Select(g => new { Year = f.Year, Genre = g.Description, Rating = f.VoteAverage, film = f }))
                                .GroupBy(g => new { g.Year, g.Genre })
                                .Select(group => new AverageByGenreYear
                                {
                                    Year = group.Key.Year,
                                    Genre = group.Key.Genre,
                                    Data = query
                                            .Where(f => f.Year == group.Key.Year && f.Genre.Any(g => g.Description == group.Key.Genre))
                                            .Include(x => x.Genre)
                                            .Include(x => x.Streaming)
                                            .Include(x => x.Reviews)
                                            .OrderBy(x => x.Title)
                                            .ToList(),
                                    AverageRating = (int)Math.Round(group.Average(g => g.Rating))
                                })
                                .ToListAsync();

            Data = Data.OrderByDescending(x => x.Year).ToList();

            return Data;
        }

        public async Task<List<Film>> ConsultFilmByRatingAsync(int Average, int pageNumber = 0, int PageSize = 20)
        {
            return await _dbContext.Film.Where(x => x.VoteAverage.Equals(Average)).OrderBy(x => x.Title).Include(x => x.Genre).Include(x => x.Streaming).Include(x => x.Reviews).Skip(pageNumber * PageSize).Take(PageSize).ToListAsync();
        }

        public async Task<Film> CreateFilmAsync(Film film)
        {
            await _dbContext.Film.AddAsync(film);

            if (film.Genre != null && film.Genre.Any())
            {
                foreach (var item in film.Genre)
                {
                    var consultGenre = await _dbContext.GenericValues.AsNoTracking().FirstOrDefaultAsync(x => x.Id == item.Id);
                    if (consultGenre != null)
                        _dbContext.Entry(item).CurrentValues.SetValues(consultGenre);
                    else
                    {
                        var value = _dbContext.GenericValues.Add(item);
                        _dbContext.Entry(item).CurrentValues.SetValues(value);
                    }
                }
            }

            if (film.Streaming != null && film.Streaming.Any())
            {
                foreach (var item in film.Streaming)
                {
                    var consultStreaming = await _dbContext.GenericValues.AsNoTracking().FirstOrDefaultAsync(x => x.Id == item.Id);
                    if (consultStreaming != null)
                        _dbContext.Entry(item).CurrentValues.SetValues(consultStreaming);
                    else
                    {
                        var value = _dbContext.GenericValues.Add(item);
                        _dbContext.Entry(item).CurrentValues.SetValues(value);
                    }
                }
            }

            if (film.Reviews != null && film.Reviews.Any())
            {
                foreach (var item in film.Reviews)
                {
                    var consultReviews = await _dbContext.Review.AsNoTracking().FirstOrDefaultAsync(x => x.Id == item.Id);
                    if (consultReviews != null)
                        _dbContext.Entry(item).CurrentValues.SetValues(consultReviews);
                    else
                    {
                        var value = _dbContext.Review.Add(item);
                        _dbContext.Entry(item).CurrentValues.SetValues(value);
                    }
                }
            }

            await _dbContext.SaveChangesAsync();
            return film;
        }

        public async Task<Film> UpdateFilmAsync(CreateFilm film)
        {
            Film? ConsultFilm = await _dbContext.Film.Include(x => x.Genre).Include(x => x.Streaming).Include(x => x.Reviews).FirstOrDefaultAsync(x => x.Id == film.Id);

            if (ConsultFilm == null)
            {
                throw new Exception($"Não foi encontrado nenhum filme com o ID: {film.Id}");
            }

            var Filtro = new Film()
            {
                Id = film.Id,
                Title = film.Title,
                Reviews = film.Reviews,
                Genre = film.Genre,
                Streaming = film.Streaming,
                Month = film.Month,
                Year = film.Year,
                VoteAverage = film.VoteAverage
            };

            Filtro.CalculateAverage();
            _dbContext.Entry(ConsultFilm).CurrentValues.SetValues(Filtro);

            ConsultFilm.Reviews?.Clear();
            ConsultFilm.Genre?.Clear();
            ConsultFilm.Streaming?.Clear();

            if (Filtro.Genre != null && Filtro.Genre.Any())
                foreach (var val in Filtro.Genre)
                {
                    var consultValue = await _dbContext.GenericValues.FirstOrDefaultAsync(x => x.Id.Equals(val.Id));
                    if (consultValue != null)
                        ConsultFilm.Genre?.Add(consultValue);
                    else
                    {
                        val.Id = 0;
                        var retValue = _dbContext.GenericValues.Add(val).Entity;
                        ConsultFilm.Genre?.Add(retValue);
                    }
                }

            if (Filtro.Streaming != null && Filtro.Streaming.Any())
                foreach (var val in Filtro.Streaming)
                {
                    var consultValue = await _dbContext.GenericValues.FirstOrDefaultAsync(x => x.Id.Equals(val.Id));
                    if (consultValue != null)
                        ConsultFilm.Streaming?.Add(consultValue);
                    else
                    {
                        val.Id = 0;
                        var retValue = _dbContext.GenericValues.Add(val).Entity;
                        ConsultFilm.Streaming?.Add(retValue);
                    }
                }

            if (Filtro.Reviews != null && Filtro.Reviews.Any())
                foreach (var val in Filtro.Reviews)
                {
                    var consultValue = await _dbContext.Review.FirstOrDefaultAsync(x => x.Id.Equals(val.Id));
                    if (consultValue != null)
                        ConsultFilm.Reviews?.Add(consultValue);
                    else
                    {
                        val.Id = 0;
                        var retValue = _dbContext.Review.Add(val).Entity;
                        ConsultFilm.Reviews?.Add(retValue);
                    }
                }

            await _dbContext.SaveChangesAsync();

            return ConsultFilm;
        }
        public async Task<bool> DeleteAsync(int id)
        {
            Film? ConsultFilm = await _dbContext.Film.Include(x => x.Genre).Include(x => x.Streaming).Include(x => x.Reviews).FirstOrDefaultAsync(x => x.Id == id);

            if (ConsultFilm == null)
                return false;


            _dbContext.Film.Remove(ConsultFilm);

            _dbContext.Database.ExecuteSqlRaw("DELETE FROM GenericValues WHERE FilmId IS NULL AND FilmId1 IS NULL");

            await _dbContext.SaveChangesAsync();

            return true;
        }

    }
}