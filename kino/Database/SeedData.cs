using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using kino.Crypto;
using System.Linq;

namespace kino.Database
{
    public class SeedData
    {
        private readonly KinoContext context;
        private readonly SignInManager<User> signInManager;
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IEncoder encoder;

        public SeedData(KinoContext context,
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager, IEncoder encoder)
        {
            this.context = context;
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.encoder = encoder;
        }

        public void EnsurePopulated()
        {
            EnsureRolesPresent();
            EnsureAdminPresent();
            EnsureMoviesPresent();
            EnsureScreeningRoomsPresent();
        }

        private void EnsureRolesPresent()
        {
            var availableRoleNames = roleManager.Roles.Select(role => role.Name).ToList();
            foreach (string roleName in Role.AllRoles)
            {
                if (!availableRoleNames.Contains(roleName))
                {
                    var role = new IdentityRole(roleName);
                    roleManager.CreateAsync(role).Wait();
                }
            }
        }

        private string EnsureAdminPresent()
        {
            User admin = signInManager.UserManager.FindByNameAsync(encoder.Encode("admin")).Result;
            if (admin == null)
            {
                var user = new User
                {
                    UserName = encoder.Encode("admin"),
                    FullName = encoder.Encode("Witold Tomaszewski"),
                };
                var createResult = signInManager.UserManager.CreateAsync(user, "Zaq12wsx!").Result;
                if (!createResult.Succeeded)
                {
                    throw new System.Exception("Cannot create administrator user!: " + createResult.Errors.First());
                }

                var roleResult = userManager.AddToRoleAsync(user, Role.Administrator).Result;
                if (!roleResult.Succeeded)
                {
                    throw new System.Exception("Cannot assign role!: " + createResult.Errors.First());
                }
                context.Entry(user).State = EntityState.Modified;
                context.SaveChanges();
                return user.Id;
            }
            return admin.Id;
        }

        private void EnsureMoviesPresent()
        {
            if (context.Movies.Count() != 0) return;
            context.AddRange(new Movie
            {
                Title = "Paradoks kłamcy rozstrzygnięty",
                Genres = "Science fiction, Dramat",
                Time = 15
            }, new Movie
            {
                Title = "Fraktale z reaktora",
                Genres = "Naukowy, Fraktale",
                Time = 130
            }, new Movie
            {
                Title = "Laboratorium z algo-rytmów",
                Genres = "Musical",
                Time = 14
            }, new Movie
            {
                Title = "Czy widzimy piąty wymiar",
                Genres = "Religia, Fraktale",
                Time = 50
            }, new Movie
            {
                Title = "Gliwice miasto wymarłe",
                Genres = "Postapokaliptyczny",
                Time = 50
            }, new Movie
            {
                Title = "Fractals",
                Genres = "Fraktale",
                Time = 235
            });
            context.SaveChanges();
        }

        private void EnsureScreeningRoomsPresent()
        {
            if (context.ScreeningRooms.Count() != 0) return;
            context.AddRange(new ScreeningRoom
            {
                Name = "Wodymidaj",
                Width = 4,
                Height = 3,
            }, new ScreeningRoom
            {
                Name = "Tets",
                Width = 20,
                Height = 9,
            });
            context.SaveChanges();
        }
    }
}
