using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ModsDude.Server.Application.Users;
using ModsDude.Server.Domain.Users;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ModsDude.Server.Application.Services;
public class JwtService : IJwtService
{
    private readonly UsersOptions _options;


    public JwtService(IOptions<UsersOptions> options)
    {
        _options = options.Value;
    }


    public string GenerateForUser(UserId userId)
    {
        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier, userId.Value.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _options.JwtSigningKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddSeconds(_options.JwtLifetimeInSeconds),
            signingCredentials: credentials
        );
        var handler = new JwtSecurityTokenHandler();

        return handler.WriteToken(token);
    }
}
