using AuthAPI.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace AuthAPI.Tests.Helpers;

/// <summary>
/// Helper class for setting up mocks commonly used in AuthAPI unit tests.
/// </summary>
public static class AuthApiMockHelper
{
    /// <summary>
    /// Creates a mock UserManager for ApplicationUser.
    /// </summary>
    public static Mock<UserManager<ApplicationUser>> CreateMockUserManager()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
        var userManager = new Mock<UserManager<ApplicationUser>>(
            store.Object, null, null, null, null, null, null, null, null);
        userManager.Object.UserValidators.Add(new UserValidator<ApplicationUser>());
        userManager.Object.PasswordValidators.Add(new PasswordValidator<ApplicationUser>());
        return userManager;
    }

    /// <summary>
    /// Creates a mock SignInManager for ApplicationUser.
    /// </summary>
    public static Mock<SignInManager<ApplicationUser>> CreateMockSignInManager(
        Mock<UserManager<ApplicationUser>> mockUserManager)
    {
        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        var httpContext = new Mock<HttpContext>();
        httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext.Object);

        var signInManager = new Mock<SignInManager<ApplicationUser>>(
            mockUserManager.Object,
            httpContextAccessor.Object,
            new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>().Object,
            null, null, null, null);
        return signInManager;
    }

    /// <summary>
    /// Creates a mock RoleManager for IdentityRole.
    /// </summary>
    public static Mock<RoleManager<IdentityRole>> CreateMockRoleManager()
    {
        var store = new Mock<IRoleStore<IdentityRole>>();
        var roleManager = new Mock<RoleManager<IdentityRole>>(
            store.Object, null, null, null, null);
        roleManager.Object.RoleValidators.Add(new RoleValidator<IdentityRole>());
        return roleManager;
    }

    /// <summary>
    /// Creates a test ApplicationUser with default values.
    /// </summary>
    public static ApplicationUser CreateTestUser(
        string id = "test-user-1",
        string email = "test@example.com",
        string userName = "testuser",
        string fullName = "Test User")
    {
        return new ApplicationUser
        {
            Id = id,
            Email = email,
            UserName = userName,
            FullName = fullName,
            EmailConfirmed = true,
            CreatedAtUtc = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Creates a test IdentityRole with default values.
    /// </summary>
    public static IdentityRole CreateTestRole(
        string id = "role-1",
        string name = "User")
    {
        return new IdentityRole
        {
            Id = id,
            Name = name,
            NormalizedName = name.ToUpper()
        };
    }
}

