using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using kino.Database;
using kino.ViewModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kino.Controllers
{
    [Route("reservation")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        private readonly KinoContext context;

        public ReservationsController(UserManager<User> userManager, KinoContext context)
        {
            this.userManager = userManager;
            this.context = context;
        }

        [HttpGet]
        [Route("")]
        [Authorize(Roles = Role.Employee, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GetScreenings()
        {
            return new OkObjectResult(context.Screenings.Include(s => s.ScreeningRoom).Include(s => s.Movie));
        }

        [HttpPost]
        [Route("{id}")]
        [Authorize(Roles = Role.User, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> AddReservation([FromForm] ReservationViewModel viewModel)
        {
            var user = await userManager.GetUserAsync(User);
            var screening = context.Screenings.FirstOrDefault(sc => sc.ScreeningId == viewModel.ScreeningId);
            if (screening == null)
            {
                return NotFound();
            }

            var reservation = new Reservation
            {
                ScreeningId = screening.ScreeningId,
                UserId = user.Id,
            };

            var movie = context.Movies.FirstOrDefault(m => m.MovieId == screening.MovieId);
            if (movie == null)
            {
                return NotFound();
            }
            var screeningRoom = context.ScreeningRooms.FirstOrDefault(sr => sr.ScreeningRoomId == screening.ScreeningRoomId);
            if (screeningRoom == null)
            {
                return NotFound();
            }
            if (viewModel.SeatX < 0 || viewModel.SeatY > 0
                || viewModel.SeatX >= screeningRoom.Width || viewModel.SeatY <= screeningRoom.Height)
            {
                return BadRequest();
            }

            context.Add(reservation);

            await context.SaveChangesAsync();

            reservation.User = user;
            reservation.Screening = screening;
            reservation.Screening.Movie = movie;
            reservation.Screening.ScreeningRoom = screeningRoom;

            return new OkObjectResult(reservation);
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = Role.User, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            var reservation = context.Reservations.FirstOrDefault(s => s.ReservationId == id);
            if (reservation == null)
            {
                return NotFound();
            }

            context.Remove(reservation);

            await context.SaveChangesAsync();

            return new OkObjectResult(reservation);
        }
    }
}
