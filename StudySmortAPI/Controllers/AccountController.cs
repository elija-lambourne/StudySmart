using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using StudySmortAPI.Model;

namespace StudySmortAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly DataContext _dbContext;

    public AccountController(DataContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Register(UserRegistrationModel model)
    {
        if (_dbContext.Users.Any(u => u.Email == model.Email))
        {
            return BadRequest("User already exists.");
        }

        // Create a new user
        var newUser = new User
        {
            Id = Guid.NewGuid(),
            Email = model.Email,
            Password = model.Password
        };

        _dbContext.Users.Add(newUser);
        _dbContext.SaveChanges();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(RandomKeyGenerator.Generate256BitKeyString()));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, newUser.Id.ToString()),
        };
        var token = new JwtSecurityToken(
            issuer: "htl-leonding",
            audience: "htl-leonding-aud",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1), // Token expiration time
            signingCredentials: credentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(new UserData(newUser.Id,tokenString));
    }
    
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Login(UserLoginModel model)
    {
        var user = _dbContext.Users.SingleOrDefault(u => u.Email == model.Email && u.Password == model.Password);

        if (user == null)
        {
            return Unauthorized("Invalid email or password.");
        }

        // Create claims for the user
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(RandomKeyGenerator.Generate256BitKeyString()));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "htl-leonding",
            audience: "htl-leonding-aud",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1), // Token expiration time
            signingCredentials: credentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(new UserData(user.Id,tokenString));
    }
}