using kino.Database;
using Microsoft.AspNetCore.Identity;
using Moq;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

    public static class UserManagerMock
    {
        public static Mock<UserManager<User>> CreateMock(List<User> users)
        {
            var store = new Mock<IUserStore<User>>();
            var userManager = new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);
            userManager.Object.UserValidators.Add(new UserValidator<User>());
            userManager.Object.PasswordValidators.Add(new PasswordValidator<User>());

            userManager.Setup(x => x.DeleteAsync(It.IsAny<User>()))
                .ReturnsAsync(IdentityResult.Success);

            userManager.Setup(x => x.CreateAsync(It.IsAny<User>(),
                It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success)
                .Callback<User, string>((x, y) => users.Add(x));

            userManager.Setup(x => x
                .UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync(IdentityResult.Success);

            userManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .Returns(Task.FromResult(new User() { Id = "1" }));

            userManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .Returns(Task.FromResult(new User { Id = "1" }));

            return userManager;
        }

        public static Mock<UserManager<User>> CreateMock()
        {
            var users = new List<User>
            {
                new User { Email="witold.tomaszewski@polsl.pl" }
            };

            return CreateMock(users);
        }
    }
