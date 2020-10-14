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
    [Route("screening-room")]
    [ApiController]
    public class ScreeningRoomsController : ControllerBase
    {
        private readonly KinoContext context;

        public ScreeningRoomsController(KinoContext context)
        {
            this.context = context;
        }

        [HttpGet]
        [Route("")]
        [Authorize(Roles = Role.Employee, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GetScreeningRooms()
        {
            return new OkObjectResult(context.ScreeningRooms);
        }

        [HttpPost]
        [Authorize(Roles = Role.Employee, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> AddScreeningRoom([FromForm] ScreeningRoom room)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (context.ScreeningRooms.FirstOrDefault(sm => sm.Name== room.Name) != null)
            {
                return Conflict();
            }

            context.Add(room);

            await context.SaveChangesAsync();

            return new OkObjectResult(room);
        }

        [HttpPatch]
        [Route("{id}")]
        [Authorize(Roles = Role.Employee, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> PatchRoom(int id, [FromForm] ScreeningRoom room)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var existingRoom = context.ScreeningRooms.FirstOrDefault(sm => sm.Name == room.Name);
            if (existingRoom == null)
            {
                return NotFound();
            }
            if (room.ScreeningRoomId != id)
            {
                return BadRequest();
            }

            existingRoom.Patch(room);

            context.Update(existingRoom);

            await context.SaveChangesAsync();

            return new OkObjectResult(room);
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = Role.Employee, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> DeleteScreeningRoom(int id)
        {
            var room = context.ScreeningRooms.FirstOrDefault(m => m.ScreeningRoomId == id);
            if (room == null)
            {
                return NotFound();
            }

            context.Remove(room);

            await context.SaveChangesAsync();

            return new OkObjectResult(room);
        }
    }
}
