using StreamBerryAPI.Models;

namespace StreamBerryAPI.Repository.Interface
{
    public interface IFilmRepository
    {
        Task<RetPaged<Film>> ListFilm(int skip = 0, int take = 20);

        Task<List<Film>> ConsultFilmByTitle(string Title, int pageNumber = 0, int PageSize = 20);

        Task<List<Film>> ConsultFilmByRating(int Average, int pageNumber = 0, int PageSize = 20);

        Task<GenericModelByYear<Film>> ConsultFilmByYear(int Year, int pageNumber = 0, int PageSize = 20);

        Task<Film> UpdateFilm(CreateFilm film);

        Task<Film> CreateFilm(Film film);

        Task<bool> Delete(int id);
    }
}
