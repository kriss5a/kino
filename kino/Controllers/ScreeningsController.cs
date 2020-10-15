using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using kino.Database;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kino.Controllers
{
    [Route("screening")]
    [ApiController]
    public class ScreeningController : ControllerBase
    {
        private readonly KinoContext context;

        public ScreeningController(KinoContext context)
        {
            this.context = context;
        }

        [HttpGet]
        [Route("")]
        [Authorize(Roles = Role.Employee + "," + Role.User, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GetScreenings()
        {
            return new OkObjectResult(context.Screenings.Include(s => s.ScreeningRoom).Include(s => s.Movie));
        }

        [HttpPost]
        [Authorize(Roles = Role.Employee, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> AddScreening([FromForm] Screening screening)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var movie = context.Movies.FirstOrDefault(m => m.MovieId == screening.MovieId);
            if (movie == null)
            {
                return NotFound();
            }
            screening.Movie = movie;
            var screeningRoom = context.ScreeningRooms.FirstOrDefault(sr => sr.ScreeningRoomId == screening.ScreeningRoomId);
            if (screeningRoom == null)
            {
                return NotFound();
            }
            screening.ScreeningRoom = screeningRoom;

            context.Add(screening);

            await context.SaveChangesAsync();

            return new OkObjectResult(screening);
        }

        [HttpPatch]
        [Route("{id}")]
        [Authorize(Roles = Role.Employee, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> PatchScreening(int id, [FromForm] Screening screening)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var existingScreening = context.Screenings.FirstOrDefault(s => s.ScreeningId == screening.ScreeningId);
            if (existingScreening == null)
            {
                return NotFound();
            }

            var existingMovie = context.Movies.FirstOrDefault(m => m.MovieId == screening.MovieId);
            if (existingMovie == null)
            {
                return NotFound();
            }
            screening.Movie = existingMovie;

            var existingScreeningRoom = context.ScreeningRooms.FirstOrDefault(sr => sr.ScreeningRoomId == sr.ScreeningRoomId);
            if (screening.ScreeningId != id)
            {
                return BadRequest();
            }
            screening.ScreeningRoom = existingScreeningRoom;

            existingScreening.Patch(screening);

            context.Update(existingScreening);

            await context.SaveChangesAsync();

            return new OkObjectResult(screening);
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = Role.Employee, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> DeleteScreening(int id)
        {
            var screening = context.Screenings.FirstOrDefault(s => s.ScreeningId== id);
            if (screening == null)
            {
                return NotFound();
            }

            context.Remove(screening);

            await context.SaveChangesAsync();

            return new OkObjectResult(screening);
        }
    }
}
