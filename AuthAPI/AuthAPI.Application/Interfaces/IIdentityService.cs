using Microsoft.AspNetCore.Http;

namespace AuthAPI.Application.Interfaces
{
    public interface IIdentityService
    {
        // User section
        Task<IResult> CreateUserAsync(string userName, string password, string email, string fullName, List<string> roles);
        Task<IResult> SigninUserAsync(string userName, string password);
        Task<IResult> GetUserIdAsync(string userName);
        Task<IResult> GetUserDetailsAsync(string userId);
        Task<IResult> GetUserDetailsByUserNameAsync(string userName);
        Task<IResult> GetUserNameAsync(string userId);
        Task<IResult> DeleteUserAsync(string userId);
        Task<IResult> IsUniqueUserName(string userName);
        Task<List<(string id, string fullName, string userName, string email)>> GetAllUsersAsync();
        Task<List<(string id, string userName, string email, IList<string> roles)>> GetAllUsersDetailsAsync();
        Task<IResult> UpdateUserProfile(string id, string fullName, string email, IList<string> roles);

        // Role Section
        Task<IResult> CreateRoleAsync(string roleName);
        Task<IResult> DeleteRoleAsync(string roleId);
        Task<List<(string id, string roleName)>> GetRolesAsync();
        Task<IResult> GetRoleByIdAsync(string id);
        Task<IResult> UpdateRole(string id, string roleName);

        // User's Role section
        Task<IResult> IsInRoleAsync(string userId, string role);
        Task<IResult> GetUserRolesAsync(string userId);
        Task<IResult> AssignUserToRole(string userName, IList<string> roles);
        Task<IResult> UpdateUsersRole(string userName, IList<string> usersRole);


    }
}
