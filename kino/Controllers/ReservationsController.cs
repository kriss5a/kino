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
using System;

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

        [HttpGet]
        [Route("seats")]
        [Authorize(Roles = Role.User, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GetTakenSeats([FromQuery] int screeningId)
        {
            return new OkObjectResult(context.Reservations.Where(r => r.ScreeningId == screeningId).Select(r => new { X = r.SeatX, Y = r.SeatY, IsTaken = !r.Expiration.HasValue }));
        }

        [HttpPost]
        [Route("confirm")]
        [Authorize(Roles = Role.User, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ConfirmReservation([FromQuery] int reservationId)
        {
            var reservation = context.Reservations.FirstOrDefault(r => r.ReservationId == reservationId);
            if (reservation == null)
            {
                return NotFound();
            }
            reservation.Expiration = null;
            reservation.IsConfirmed = true;
            context.Update(reservation);
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        [Route("start")]
        [Authorize(Roles = Role.User, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> StartReservation([FromBody] StartReservationViewModel viewModel)
        {
            var user = await userManager.GetUserAsync(User);
            var screening = context.Screenings.FirstOrDefault(sc => sc.ScreeningId == viewModel.ScreeningId);
            if (screening == null)
            {
                return NotFound();
            }
            var reservation = context.Reservations.FirstOrDefault(r => r.ScreeningId == viewModel.ScreeningId && r.SeatX == viewModel.X && r.SeatY == viewModel.Y);
            if (reservation == null)
            {
                var newReservation = new Reservation
                {
                    Expiration = DateTime.Now.AddMinutes(1),
                    ScreeningId = viewModel.ScreeningId,
                    SeatX = viewModel.X,
                    SeatY = viewModel.Y,
                    UserId = user.Id,
                    Priority = 0,
                };
                context.Reservations.Add(newReservation);
                await context.SaveChangesAsync();

                return new OkObjectResult(new StartReservationResponse
                {
                    Status = "OK",
                    Reservation = newReservation,
                });
            }
            if (!reservation.Expiration.HasValue)
            {
                return new OkObjectResult(new StartReservationResponse
                {
                    Status = "TAKEN",
                    Reservation = null,
                });
            }
            else
            {
                var priority = context.Reservations
                    .Where(r => r.ScreeningId == viewModel.ScreeningId && r.SeatX == viewModel.X && r.SeatY == viewModel.Y)
                    .Select(r => r.Priority)
                    .Max() + 1;
                var newReservation = new Reservation
                {
                    ScreeningId = viewModel.ScreeningId,
                    SeatX = viewModel.X,
                    SeatY = viewModel.Y,
                    UserId = user.Id,
                    Priority = priority,
                };
                context.Reservations.Add(newReservation);
                await context.SaveChangesAsync();

                return new OkObjectResult(new StartReservationResponse
                {
                    Status = "WAIT",
                    Reservation = newReservation,
                });
            }
        }

        [HttpPost]
        [Route("check")]
        [Authorize(Roles = Role.User, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> CheckReservation([FromBody] int reservationId)
        {
            var myReservation = context.Reservations.FirstOrDefault(r => r.ReservationId == reservationId);
            if (myReservation == null)
            {
                return new OkObjectResult("NOPE NA AMEN");
            }
            if (myReservation.Expiration.HasValue)
            {
                return BadRequest(); // nie warto sprawdzać potwierdzonej rezerwacji
            }

            var otherReservations = context.Reservations
                .Where(r => r.ScreeningId == myReservation.ScreeningId && r.SeatX == myReservation.SeatX && r.SeatY == myReservation.SeatY);

            var confirmed = await otherReservations.Where(r => r.IsConfirmed).FirstOrDefaultAsync(); // no niestety
            if (confirmed != null)
            {
                return new OkObjectResult("NOPE NA AMEN");
            }

            var leader = await otherReservations.Where(r => r.Expiration.HasValue).FirstOrDefaultAsync();
            if (leader == null) // leader sobie poszedł
            {
                var min = otherReservations.Select(r => r.Priority).Min(); // wybieramy z najniższą wartością
                if (myReservation.Priority == min) // to my!
                {
                    myReservation.Expiration = DateTime.Now.AddMinutes(1);
                    context.Update(myReservation);
                    await context.SaveChangesAsync();
                    return new OkObjectResult(myReservation);
                }
                else
                {
                    return new OkObjectResult("NOPE"); // nie my, ale warto dalej próbować
                }
            }
            else // leader istnieje
            {
                if (leader.Expiration.Value > DateTime.Now) // i dalej ma prawo wyboru
                {
                    return new OkObjectResult("NOPE");
                }
                else
                {
                    context.Remove(leader); // leader is dead!
                    await context.SaveChangesAsync();

                    var min = otherReservations.Select(r => r.Priority).Min(); // wybieramy z najniższ wartości
                    if (myReservation.Priority == min)
                    {
                        myReservation.Expiration = DateTime.Now.AddMinutes(1);
                        context.Update(myReservation);
                        await context.SaveChangesAsync();
                        return new OkObjectResult(myReservation);
                    }
                    else
                    {
                        if (leader.IsConfirmed)// jest rezerwacja zaklepana
                        {
                            var res = otherReservations.Where(r => !r.IsConfirmed);
                            context.RemoveRange(res);
                            await context.SaveChangesAsync();
                            return new OkObjectResult("NOPE NA AMEN");
                        }
                        else
                        {
                            return new OkObjectResult("NOPE"); // nie my, ale warto dalej próbować
                        }
                    }
                }
            }
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

    public class StartReservationResponse
    {
        public string Status { get; set; }
        public Reservation Reservation { get; set; }
    }

    public class StartReservationViewModel
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int ScreeningId { get; set; }
    }
}
