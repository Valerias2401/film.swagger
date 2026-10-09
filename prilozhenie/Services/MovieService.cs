using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prilozhenie.DatabaseContext;
using prilozhenie.Interfaces;
using prilozhenie.Models;
using prilozhenie.Requests;

namespace prilozhenie.Services
{
    public class MovieService : IMovieService
    {
        private readonly ContextDb _context;

        public MovieService(ContextDb context) => _context = context;

        public async Task<IActionResult> GetAllMoviesAsync()
        {
            var movies = await _context.Movies
                .Include(m => m.Genre)
                .Select(m => new
                {
                    m.MovieId,
                    m.Title,
                    m.Description,
                    Genre = m.Genre!.GenreName,
                    m.ReleaseYear,
                    m.Rating,
                    m.PosterUrl
                })
                .ToListAsync();

            return new OkObjectResult(new { status = true, movies });
        }

        public async Task<IActionResult> GetMovieByIdAsync(int id)
        {
            var movie = await _context.Movies
                .Include(m => m.Genre)
                .FirstOrDefaultAsync(m => m.MovieId == id);

            if (movie == null)
                return new NotFoundObjectResult(new { status = false, message = "Фильм не найден" });

            return new OkObjectResult(new
            {
                status = true,
                movie = new
                {
                    movie.MovieId,
                    movie.Title,
                    movie.Description,
                    Genre = movie.Genre!.GenreName,
                    movie.ReleaseYear,
                    movie.Rating,
                    movie.PosterUrl
                }
            });
        }

        public async Task<IActionResult> AddMovieAsync(MovieRequest request, int adminUserId)
        {
            // 🔐 Проверка: только админ может добавлять
            var admin = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id_User == adminUserId);

            if (admin?.Role?.RoleName != "Admin")
                return new ForbidResult();

            var genre = await _context.Genres.FindAsync(request.GenreId);
            if (genre == null)
                return new BadRequestObjectResult(new { status = false, message = "Жанр не найден" });

            var movie = new Movie
            {
                Title = request.Title,
                Description = request.Description,
                GenreId = request.GenreId.Value,
                ReleaseYear = request.ReleaseYear,
                Rating = request.Rating,
                PosterUrl = request.PosterUrl
            };

            await _context.Movies.AddAsync(movie);
            await _context.SaveChangesAsync();

            return new OkObjectResult(new { status = true, message = "Фильм добавлен", movieId = movie.MovieId });
        }

        public async Task<IActionResult> UpdateMovieAsync(int id, MovieRequest request, int adminUserId)
        {
            var admin = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id_User == adminUserId);

            if (admin?.Role?.RoleName != "Admin")
                return new ForbidResult();

            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
                return new NotFoundObjectResult(new { status = false, message = "Фильм не найден" });

            if (!string.IsNullOrEmpty(request.Title)) movie.Title = request.Title;
            if (request.Description != null) movie.Description = request.Description;
            if (request.GenreId.HasValue) movie.GenreId = request.GenreId.Value;
            if (request.ReleaseYear > 0) movie.ReleaseYear = request.ReleaseYear;
            if (request.Rating >= 0) movie.Rating = request.Rating;
            if (request.PosterUrl != null) movie.PosterUrl = request.PosterUrl;

            await _context.SaveChangesAsync();
            return new OkObjectResult(new { status = true, message = "Фильм обновлён" });
        }

        public async Task<IActionResult> DeleteMovieAsync(int id, int adminUserId)
        {
            var admin = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id_User == adminUserId);

            if (admin?.Role?.RoleName != "Admin")
                return new ForbidResult();

            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
                return new NotFoundObjectResult(new { status = false, message = "Фильм не найден" });

            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();

            return new OkObjectResult(new { status = true, message = "Фильм удалён" });
        }

        public async Task<IActionResult> GetGenresAsync()
        {
            var genres = await _context.Genres
                .Select(g => new { g.GenreId, g.GenreName })
                .ToListAsync();

            return new OkObjectResult(new { status = true, genres });
        }
    }
}