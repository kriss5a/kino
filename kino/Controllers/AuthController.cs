using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using kino;
using kino.Crypto;
using kino.Database;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using kino.ViewModel;

namespace KnmBackend.Controllers
{

    [Route("auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<User> signInManager;
        private readonly UserManager<User> userManager;
        private readonly KinoContext context;
        private readonly IEncoder encoder;
        private readonly JwtTokenGenerator tokenGenerator;

        public AuthController(SignInManager<User> signInManager, UserManager<User> userManager, KinoContext context, IEncoder encoder, JwtTokenGenerator tokenGenerator)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.context = context;
            this.encoder = encoder;
            this.tokenGenerator = tokenGenerator;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromForm] LoginViewModel loginDetails)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await userManager.FindByNameAsync(encoder.Encode(loginDetails.Username));
            if (user != null)
            {
                await signInManager.SignOutAsync();

                var result = await signInManager.PasswordSignInAsync(user, loginDetails.Password, false, false);
                if (result.Succeeded)
                {
                    var token = tokenGenerator.GenerateToken(user);
                    if (result.Succeeded)
                    {
                        var u = await context.Users.FirstOrDefaultAsync(u => u.UserName == encoder.Encode(loginDetails.Username));
                        if (u == null)
                        {
                            return BadRequest("User does not exist");
                        }

                        var roles = await userManager.GetRolesAsync(u);

                        return new OkObjectResult(new LoginResultViewModel
                        {
                            User = new UserViewModel(u, encoder, roles),
                            Token = token,
                        });
                    }
                    else
                    {
                        return Forbid();
                    }
                }
            }
            return Forbid();
        }

        [HttpPost]
        [Authorize(Roles = Role.Administrator, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("register")]
        public async Task<ActionResult<UserViewModel>> Register([FromForm] RegisterViewModel registerDetails)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!Role.AllRoles.Contains(registerDetails.Role))
            {
                return BadRequest();
            }
            if (!CheckPasswordRequirements(registerDetails.Password))
            {
                return BadRequest();
            }

            var resultUser = new User
            {
                UserName = encoder.Encode(registerDetails.Username),
                FullName = encoder.Encode(registerDetails.FullName),
            };

            context.Users.Add(resultUser);

            var createResult = await userManager.CreateAsync(resultUser, registerDetails.Password);
            if (!createResult.Succeeded)
            {
                return BadRequest();
            }
            var roleResult = await userManager.AddToRoleAsync(resultUser, registerDetails.Role);
            if (!roleResult.Succeeded)
            {
                return BadRequest();
            }

            context.Entry(resultUser).State = EntityState.Modified;
            await context.SaveChangesAsync();
            
            if (resultUser != null)
            {
                return new OkObjectResult(new UserViewModel(resultUser, encoder, registerDetails.Role));
            }
            else
            {
                return BadRequest();
            }
        }

        public static string GetMaximalRole(IList<string> roles)
        {
            if (roles.Contains(Role.Administrator))
            {
                return Role.Administrator;
            }
            else if (roles.Contains(Role.Employee))
            {
                return Role.Employee;
            }
            return Role.User;
        }
        
        [HttpPost]
        [Authorize(Roles = Role.Employee, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("change-password")]
        public async Task<ActionResult> ChangePassword([FromForm] ChangePasswordViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!CheckPasswordRequirements(viewModel.NewPassword))
            {
                return BadRequest();
            }

            var user = await userManager.GetUserAsync(User);

            var signInResult = await signInManager.PasswordSignInAsync(user, viewModel.CurrentPassword, false, false);
            if (signInResult.Succeeded)
            {
                string newPassword = userManager.PasswordHasher.HashPassword(user, viewModel.NewPassword);
                user.PasswordHash = newPassword;
                IdentityResult updateResult = await userManager.UpdateAsync(user);
                await signInManager.SignOutAsync();
                if (updateResult.Succeeded)
                {
                    return new OkObjectResult(null);
                }
            }
            return BadRequest();
        }
        private bool CheckPasswordRequirements(string password)
        {
            return password.Length >= 6 &&
                Regex.IsMatch(password, "[A-Z]") &&
                Regex.IsMatch(password, "[a-z]") &&
                Regex.IsMatch(password, "\\d") &&
                Regex.IsMatch(password, "[!@#$%^&*()_+{}:<>?]");
        }
    }
}