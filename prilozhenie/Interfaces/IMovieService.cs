using Microsoft.AspNetCore.Mvc;
using prilozhenie.Requests;

namespace prilozhenie.Interfaces
{
    public interface IMovieService
    {
        Task<IActionResult> GetAllMoviesAsync();
        Task<IActionResult> GetMovieByIdAsync(int id);
        Task<IActionResult> AddMovieAsync(MovieRequest request, int adminUserId);
        Task<IActionResult> UpdateMovieAsync(int id, MovieRequest request, int adminUserId);
        Task<IActionResult> DeleteMovieAsync(int id, int adminUserId);
        Task<IActionResult> GetGenresAsync();
    }
}