using Microsoft.AspNetCore.Mvc;
using prilozhenie.Interfaces;
using prilozhenie.Requests;

namespace prilozhenie.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovieController : ControllerBase
    {
        private readonly IMovieService _movieService;

        public MovieController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMovies()
            => await _movieService.GetAllMoviesAsync();

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMovie(int id)
            => await _movieService.GetMovieByIdAsync(id);

        [HttpPost]
        public async Task<IActionResult> AddMovie([FromBody] MovieRequest request)
        {
            if (!int.TryParse(Request.Headers["X-User-Id"].FirstOrDefault(), out var adminId))
                return new UnauthorizedObjectResult(new { status = false, message = "Требуется авторизация" });

            return await _movieService.AddMovieAsync(request, adminId);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMovie(int id, [FromBody] MovieRequest request)
        {
            if (!int.TryParse(Request.Headers["X-User-Id"].FirstOrDefault(), out var adminId))
                return new UnauthorizedObjectResult(new { status = false, message = "Требуется авторизация" });

            return await _movieService.UpdateMovieAsync(id, request, adminId);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            if (!int.TryParse(Request.Headers["X-User-Id"].FirstOrDefault(), out var adminId))
                return new UnauthorizedObjectResult(new { status = false, message = "Требуется авторизация" });

            return await _movieService.DeleteMovieAsync(id, adminId);
        }

        [HttpGet("genres")]
        public async Task<IActionResult> GetGenres()
            => await _movieService.GetGenresAsync();
    }
}