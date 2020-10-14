using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using kino.Database;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kino.Controllers
{
    [Route("movie")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly KinoContext context;

        public MovieController(KinoContext context)
        {
            this.context = context;
        }

        [HttpGet]
        [Route("")]
        [Authorize(Roles = Role.Employee, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GetMovies()
        {
            return new OkObjectResult(context.Movies);
        }

        [HttpPost]
        [Authorize(Roles = Role.Employee, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> AddMovie([FromForm] Movie movie)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (context.Movies.FirstOrDefault(m => m.Title == movie.Title) != null)
            {
                return Conflict();
            }

            context.Add(movie);

            await context.SaveChangesAsync();

            return new OkObjectResult(movie);
        }

        [HttpPatch]
        [Route("{id}")]
        [Authorize(Roles = Role.Employee, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> PatchMovie(int id, [FromForm] Movie movie)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var existingMovie = context.Movies.FirstOrDefault(m => m.Title == movie.Title);
            if (existingMovie == null)
            {
                return NotFound();
            }
            if (movie.MovieId != id)
            {
                return BadRequest();
            }

            existingMovie.Patch(movie);
            context.Update(existingMovie);

            await context.SaveChangesAsync();

            return new OkObjectResult(movie);
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = Role.Employee, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            var movie = context.Movies.FirstOrDefault(m => m.MovieId == id);
            if (movie == null)
            {
                return NotFound();
            }

            context.Remove(movie);

            await context.SaveChangesAsync();

            return new OkObjectResult(movie);
        }
    }
}
