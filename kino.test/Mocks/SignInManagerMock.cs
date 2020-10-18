using kino.Database;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

public static class SignInManagerMock
{
    public static Mock<SignInManager<User>> CreateMock()
    {
        var mock = new Mock<SignInManager<User>>(
        UserManagerMock.CreateMock().Object,
        new HttpContextAccessor(),
        new Mock<IUserClaimsPrincipalFactory<User>>().Object,
        new Mock<IOptions<IdentityOptions>>().Object,
        new Mock<ILogger<SignInManager<User>>>().Object,
        new Mock<IAuthenticationSchemeProvider>().Object,
        new Mock<IUserConfirmation<User>>().Object);

        return mock;
    }
}
