using System.Security.Claims;

public interface IJwtTokenService
{
    string GenerateToken(string username);
    ClaimsPrincipal? ValidateToken(string token);

}
