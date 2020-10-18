using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using kino.Crypto;
using kino.Database;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kino.Controllers
{
    [Route("user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        private readonly KinoContext context;
        private readonly IEncoder encoder;

        public UserController(UserManager<User> userManager, KinoContext context, IEncoder encoder)
        {
            this.userManager = userManager;
            this.context = context;
            this.encoder = encoder;
        }

        [HttpGet]
        [Route("")]
        [Authorize(Roles = Role.Administrator, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetUsers()
        {
            var users = context.Users;
            var vms = new List<UserViewModel>();
            foreach (var user in users)
            {
                vms.Add(new UserViewModel(user, encoder, await userManager.GetRolesAsync(user)));
            }

            return new OkObjectResult(vms);
        }

        [HttpGet]
        [Route("reservations")]
        [Authorize(Roles = Role.User, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetReservations()
        {
            var user = await userManager.GetUserAsync(User);
            var reservations = context.Reservations
                .Where(r => r.UserId == user.Id)
                .Include(r => r.Screening)
                .ThenInclude(sc => sc.Movie)
                .Include(r => r.Screening)
                .ThenInclude(sc => sc.ScreeningRoom)
                .ToArray();

            var badReservations = reservations.Where(r => r.Expiration.HasValue || !r.IsConfirmed);
            context.RemoveRange(badReservations);
            await context.SaveChangesAsync();

            return new OkObjectResult(reservations.Where(r => !r.Expiration.HasValue && r.IsConfirmed));
        }

        [HttpGet]
        [Route("{id}")]
        [Authorize(Roles = Role.Administrator, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetUser(string id)
        {
            var user = await context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return new OkObjectResult(new UserViewModel(user, encoder));
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = Role.Administrator, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            var response = new UserViewModel(user, encoder);

            context.Remove(user);

            await context.SaveChangesAsync();

            return new OkObjectResult(response);
        }

        [HttpPost]
        [Route("change-user-data")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ChangeUserData(ChangeUserDataViewModel newData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var currentUser = await userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Forbid();
            }

            currentUser.FullName = encoder.Encode(newData.FullName);

            context.Update(currentUser);

            await context.SaveChangesAsync();

            return new OkObjectResult(newData);
        }

    }
}
