using System.Security.Claims;

public interface IJwtTokenService
{
    string GenerateToken(string username, int userId, string role);
    ClaimsPrincipal ValidateToken(string token);
}
