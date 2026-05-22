using System.Security.Claims;

namespace Common.JwtHelperService.Interfaces
{
    public interface ITokenGenerator
    {
        //public string GenerateToken(string userName, string password);
        public string GenerateJwtToken((string userId, string userName, IList<Claim> roleClaims) userDetails);
    }
}
