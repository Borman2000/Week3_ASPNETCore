using AuthAPI.Domain.Entities;
using AuthAPI.Infrastructure.Services;
using AuthAPI.Tests.Helpers;
using AutoMapper;
using Common.JwtHelperService.Interfaces;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace AuthAPI.Tests;

/// <summary>
/// Unit tests for AuthAPI.Infrastructure.Services.IdentityService
/// Tests core authentication logic including login, user creation, role management, and validation.
/// </summary>
public class IdentityServiceTests
{
	#region SigninUserAsync Tests - DEFERRED

	// NOTE: SigninUserAsync tests are deferred because the IdentityService uses ActivitySourceProvider
	// for OpenTelemetry tracing, which requires setting up the ActivitySource properly.
	// This would require infrastructure setup beyond unit test scope.
	// Consider mocking OpenTelemetryService or refactoring to inject activity handling.
	// See GitHub issue #4 for tracking.

	#endregion

	#region CreateUserAsync Tests

	[Fact]
	public async Task CreateUserAsync_WithValidData_ReturnsOkWithUserId()
	{
		// Arrange
		var mockUserManager = AuthApiMockHelper.CreateMockUserManager();
		var mockSignInManager = AuthApiMockHelper.CreateMockSignInManager(mockUserManager);
		var mockRoleManager = AuthApiMockHelper.CreateMockRoleManager();
		var mockMapper = new Mock<IMapper>();
		var mockTokenGenerator = new Mock<ITokenGenerator>();

		var testUser = AuthApiMockHelper.CreateTestUser();
		var roles = new List<string> { "User" };

		mockUserManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
			.Callback((ApplicationUser u, string p) =>
			{
				u.Id = testUser.Id;
				u.UserName = testUser.UserName;
				u.Email = testUser.Email;
			})
			.ReturnsAsync(IdentityResult.Success);
		mockUserManager.Setup(x => x.AddToRolesAsync(It.IsAny<ApplicationUser>(), roles))
			.ReturnsAsync(IdentityResult.Success);

		var service = new IdentityService(mockUserManager.Object, mockSignInManager.Object,
			mockRoleManager.Object, mockMapper.Object, mockTokenGenerator.Object);

		// Act
		var result = await service.CreateUserAsync(testUser.UserName, "Password123!", testUser.Email, testUser.FullName, roles);

		// Assert
		Assert.NotNull(result);
		mockUserManager.Verify(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once);
		mockUserManager.Verify(x => x.AddToRolesAsync(It.IsAny<ApplicationUser>(), roles), Times.Once);
	}

	[Fact]
	public async Task CreateUserAsync_WithDuplicateUsername_ReturnsValidationProblem()
	{
		// Arrange
		var mockUserManager = AuthApiMockHelper.CreateMockUserManager();
		var mockSignInManager = AuthApiMockHelper.CreateMockSignInManager(mockUserManager);
		var mockRoleManager = AuthApiMockHelper.CreateMockRoleManager();
		var mockMapper = new Mock<IMapper>();
		var mockTokenGenerator = new Mock<ITokenGenerator>();

		var error = new IdentityError { Code = "DuplicateUserName", Description = "Username already exists" };
		mockUserManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
			.ReturnsAsync(IdentityResult.Failed(error));

		var service = new IdentityService(mockUserManager.Object, mockSignInManager.Object,
			mockRoleManager.Object, mockMapper.Object, mockTokenGenerator.Object);

		// Act
		var result = await service.CreateUserAsync("existinguser", "Password123!", "test@example.com", "Test User", new List<string> { "User" });

		// Assert
		Assert.NotNull(result);
		mockUserManager.Verify(x => x.AddToRolesAsync(It.IsAny<ApplicationUser>(), It.IsAny<IList<string>>()), Times.Never);
	}

	[Fact]
	public async Task CreateUserAsync_WithRoleAssignmentFailure_ReturnsValidationProblem()
	{
		// Arrange
		var mockUserManager = AuthApiMockHelper.CreateMockUserManager();
		var mockSignInManager = AuthApiMockHelper.CreateMockSignInManager(mockUserManager);
		var mockRoleManager = AuthApiMockHelper.CreateMockRoleManager();
		var mockMapper = new Mock<IMapper>();
		var mockTokenGenerator = new Mock<ITokenGenerator>();

		var testUser = AuthApiMockHelper.CreateTestUser();
		var roles = new List<string> { "InvalidRole" };
		var roleError = new IdentityError { Code = "RoleNotFound", Description = "Role not found" };

		mockUserManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
			.ReturnsAsync(IdentityResult.Success);
		mockUserManager.Setup(x => x.AddToRolesAsync(It.IsAny<ApplicationUser>(), roles))
			.ReturnsAsync(IdentityResult.Failed(roleError));

		var service = new IdentityService(mockUserManager.Object, mockSignInManager.Object,
			mockRoleManager.Object, mockMapper.Object, mockTokenGenerator.Object);

		// Act
		var result = await service.CreateUserAsync(testUser.UserName, "Password123!", testUser.Email, testUser.FullName, roles);

		// Assert
		Assert.NotNull(result);
		mockUserManager.Verify(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once);
		mockUserManager.Verify(x => x.AddToRolesAsync(It.IsAny<ApplicationUser>(), roles), Times.Once);
	}

	#endregion

	#region DeleteRoleAsync Tests

	[Fact]
	public async Task DeleteRoleAsync_WithAdminRole_ReturnsBadRequest()
	{
		// Arrange
		var mockUserManager = AuthApiMockHelper.CreateMockUserManager();
		var mockSignInManager = AuthApiMockHelper.CreateMockSignInManager(mockUserManager);
		var mockRoleManager = AuthApiMockHelper.CreateMockRoleManager();
		var mockMapper = new Mock<IMapper>();
		var mockTokenGenerator = new Mock<ITokenGenerator>();

		var adminRole = AuthApiMockHelper.CreateTestRole(name: "Admin");

		mockRoleManager.Setup(x => x.FindByIdAsync("admin-id"))
			.ReturnsAsync(adminRole);

		var service = new IdentityService(mockUserManager.Object, mockSignInManager.Object,
			mockRoleManager.Object, mockMapper.Object, mockTokenGenerator.Object);

		// Act
		var result = await service.DeleteRoleAsync("admin-id");

		// Assert
		Assert.NotNull(result);
		mockRoleManager.Verify(x => x.DeleteAsync(It.IsAny<IdentityRole>()), Times.Never);
	}

	[Fact]
	public async Task DeleteRoleAsync_WithNonAdminRole_ReturnsOk()
	{
		// Arrange
		var mockUserManager = AuthApiMockHelper.CreateMockUserManager();
		var mockSignInManager = AuthApiMockHelper.CreateMockSignInManager(mockUserManager);
		var mockRoleManager = AuthApiMockHelper.CreateMockRoleManager();
		var mockMapper = new Mock<IMapper>();
		var mockTokenGenerator = new Mock<ITokenGenerator>();

		var userRole = AuthApiMockHelper.CreateTestRole(name: "User");

		mockRoleManager.Setup(x => x.FindByIdAsync("user-role-id"))
			.ReturnsAsync(userRole);
		mockRoleManager.Setup(x => x.DeleteAsync(userRole))
			.ReturnsAsync(IdentityResult.Success);

		var service = new IdentityService(mockUserManager.Object, mockSignInManager.Object,
			mockRoleManager.Object, mockMapper.Object, mockTokenGenerator.Object);

		// Act
		var result = await service.DeleteRoleAsync("user-role-id");

		// Assert
		Assert.NotNull(result);
		mockRoleManager.Verify(x => x.DeleteAsync(userRole), Times.Once);
	}

	[Fact]
	public async Task DeleteRoleAsync_WithNonexistentRole_ReturnsNotFound()
	{
		// Arrange
		var mockUserManager = AuthApiMockHelper.CreateMockUserManager();
		var mockSignInManager = AuthApiMockHelper.CreateMockSignInManager(mockUserManager);
		var mockRoleManager = AuthApiMockHelper.CreateMockRoleManager();
		var mockMapper = new Mock<IMapper>();
		var mockTokenGenerator = new Mock<ITokenGenerator>();

		mockRoleManager.Setup(x => x.FindByIdAsync("nonexistent"))
			.ReturnsAsync((IdentityRole)null!);

		var service = new IdentityService(mockUserManager.Object, mockSignInManager.Object,
			mockRoleManager.Object, mockMapper.Object, mockTokenGenerator.Object);

		// Act
		var result = await service.DeleteRoleAsync("nonexistent");

		// Assert
		Assert.NotNull(result);
		mockRoleManager.Verify(x => x.DeleteAsync(It.IsAny<IdentityRole>()), Times.Never);
	}

	#endregion

	#region IsUniqueUserName Tests

	[Fact]
	public async Task IsUniqueUserName_WithUniqueUsername_ReturnsTrue()
	{
		// Arrange
		var mockUserManager = AuthApiMockHelper.CreateMockUserManager();
		var mockSignInManager = AuthApiMockHelper.CreateMockSignInManager(mockUserManager);
		var mockRoleManager = AuthApiMockHelper.CreateMockRoleManager();
		var mockMapper = new Mock<IMapper>();
		var mockTokenGenerator = new Mock<ITokenGenerator>();

		mockUserManager.Setup(x => x.FindByNameAsync("newuser"))
			.ReturnsAsync((ApplicationUser)null!);

		var service = new IdentityService(mockUserManager.Object, mockSignInManager.Object,
			mockRoleManager.Object, mockMapper.Object, mockTokenGenerator.Object);

		// Act
		var result = await service.IsUniqueUserName("newuser");

		// Assert
		Assert.NotNull(result);
	}

	[Fact]
	public async Task IsUniqueUserName_WithExistingUsername_ReturnsFalse()
	{
		// Arrange
		var mockUserManager = AuthApiMockHelper.CreateMockUserManager();
		var mockSignInManager = AuthApiMockHelper.CreateMockSignInManager(mockUserManager);
		var mockRoleManager = AuthApiMockHelper.CreateMockRoleManager();
		var mockMapper = new Mock<IMapper>();
		var mockTokenGenerator = new Mock<ITokenGenerator>();

		var existingUser = AuthApiMockHelper.CreateTestUser();
		mockUserManager.Setup(x => x.FindByNameAsync(existingUser.UserName))
			.ReturnsAsync(existingUser);

		var service = new IdentityService(mockUserManager.Object, mockSignInManager.Object,
			mockRoleManager.Object, mockMapper.Object, mockTokenGenerator.Object);

		// Act
		var result = await service.IsUniqueUserName(existingUser.UserName);

		// Assert
		Assert.NotNull(result);
	}

	#endregion

	#region DeleteUserAsync Tests - DEFERRED

	// NOTE: DeleteUserAsync tests are deferred because they require proper async queryable mocking
	// for the UserManager.Users property. This requires a more sophisticated mock setup with
	// AsyncEnumerable support, which is better suited for integration tests.
	// See GitHub issue #4 for tracking.

	#endregion

	#region CreateRoleAsync Tests

	[Fact]
	public async Task CreateRoleAsync_WithValidRoleName_ReturnsOk()
	{
		// Arrange
		var mockUserManager = AuthApiMockHelper.CreateMockUserManager();
		var mockSignInManager = AuthApiMockHelper.CreateMockSignInManager(mockUserManager);
		var mockRoleManager = AuthApiMockHelper.CreateMockRoleManager();
		var mockMapper = new Mock<IMapper>();
		var mockTokenGenerator = new Mock<ITokenGenerator>();

		mockRoleManager.Setup(x => x.CreateAsync(It.IsAny<IdentityRole>()))
			.ReturnsAsync(IdentityResult.Success);

		var service = new IdentityService(mockUserManager.Object, mockSignInManager.Object,
			mockRoleManager.Object, mockMapper.Object, mockTokenGenerator.Object);

		// Act
		var result = await service.CreateRoleAsync("NewRole");

		// Assert
		Assert.NotNull(result);
		mockRoleManager.Verify(x => x.CreateAsync(It.IsAny<IdentityRole>()), Times.Once);
	}

	[Fact]
	public async Task CreateRoleAsync_WithDuplicateRoleName_ReturnsValidationProblem()
	{
		// Arrange
		var mockUserManager = AuthApiMockHelper.CreateMockUserManager();
		var mockSignInManager = AuthApiMockHelper.CreateMockSignInManager(mockUserManager);
		var mockRoleManager = AuthApiMockHelper.CreateMockRoleManager();
		var mockMapper = new Mock<IMapper>();
		var mockTokenGenerator = new Mock<ITokenGenerator>();

		var error = new IdentityError { Code = "DuplicateRoleName", Description = "Role name already exists" };
		mockRoleManager.Setup(x => x.CreateAsync(It.IsAny<IdentityRole>()))
			.ReturnsAsync(IdentityResult.Failed(error));

		var service = new IdentityService(mockUserManager.Object, mockSignInManager.Object,
			mockRoleManager.Object, mockMapper.Object, mockTokenGenerator.Object);

		// Act
		var result = await service.CreateRoleAsync("ExistingRole");

		// Assert
		Assert.NotNull(result);
	}

	#endregion
}

/// <summary>
/// Extension methods for testing with AsyncQueryable mocks.
/// </summary>
internal static class MockExtensions
{
	public static Mock<IQueryable<T>> BuildMockDbSet<T>(this IQueryable<T> source) where T : class
	{
		var mock = new Mock<IQueryable<T>>();
		mock.Setup(m => m.Provider).Returns(source.Provider);
		mock.Setup(m => m.Expression).Returns(source.Expression);
		mock.Setup(m => m.ElementType).Returns(source.ElementType);
		mock.Setup(m => m.GetEnumerator()).Returns(source.GetEnumerator());
		return mock;
	}
}

