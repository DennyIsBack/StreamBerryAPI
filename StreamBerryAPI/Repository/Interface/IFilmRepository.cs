using StreamBerryAPI.Models;

namespace StreamBerryAPI.Repository.Interface
{
    public interface IFilmRepository
    {
        Task<RetPaged<Film>> ListFilmAsync(int pageNumber, int PageSize);

        Task<List<Film>> ConsultFilmByTitleAsync(string Title, int pageNumber = 0, int PageSize = 20);

        Task<List<Film>> ConsultFilmByRatingAsync(int Average, int pageNumber = 0, int PageSize = 20);

        Task<GenericModelByYear<Film>> ConsultFilmByYearAsync(int Year, int pageNumber = 0, int PageSize = 20);

        Task<FilmVoteAverageByGenre> VoteAverageByGenreYearAsync(string Genre, int Year, int pageNumber = 0, int PageSize = 20);

        Task<Film> UpdateFilmAsync(CreateFilm film);

        Task<Film> CreateFilmAsync(Film film);

        Task<bool> DeleteAsync(int id);
    }
}
