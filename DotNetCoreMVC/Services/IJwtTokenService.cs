using System.Security.Claims;

public interface IJwtTokenService
{
    string GenerateToken(string username, int userId);
    ClaimsPrincipal? ValidateToken(string token);

}
