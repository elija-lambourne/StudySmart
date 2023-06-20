using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using StudySmortAPI.Model;

namespace StudySmortAPI.Controllers;

[ApiController]
[Route("[controller]")]
public partial class AccountController : ControllerBase
{
    private readonly DataContext _dbContext;
    private const string TokenSecret = "sjkhwlakejh2kljh23kjh4kjndsakjfkjh43kjh";
    public AccountController(DataContext dbContext, ILogger<AccountController> logger)
    {
        _dbContext = dbContext;
    }

    [HttpPut]
    [Authorize]
    public IActionResult UpdateData(UserData data)
    {
        var userId = GetGuidFromToken(HttpContext);
        if (userId == null)
        {
            return BadRequest();
        }

        var user = _dbContext.Users.FirstOrDefault(u => u.Id == userId);
        if (user == null)
        {
            return Unauthorized("User has been deleted but JWT is still valid");
        }

        user.Image = data.Pfp;
        user.Email = data.Email;
        user.Username = data.Username;

        _dbContext.SaveChangesAsync();
        
        return Ok();
    }
    
    [HttpGet]
    [Authorize]
    public IActionResult GetUserData()
    {
        var userId = GetGuidFromToken(HttpContext);
        if (userId == null)
        {
            return BadRequest();
        }

        var user = _dbContext.Users.FirstOrDefault(u => u.Id == userId);
        if (user == null)
        {
            return Unauthorized("User has been deleted but JWT is still valid");
        }

        return Ok(new UserData(userId.ToString(), user.Username, user.Email, string.Empty, user.Image));
    }

    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Register(UserRegistrationModel model)
    {        
        if (!MyRegex().IsMatch(model.Email) || _dbContext.Users.Any(u => u.Email == model.Email))
        {
            return BadRequest("User already exists.");
        }
        
        var newUser = new User
        {
            Id = Guid.NewGuid(),
            Email = model.Email,
            Password = model.Password,
            Username = model.Username,
            Image = model.Image
        };

        var rootDir = new Folder()
        {
            ChildFolders = new List<Folder>(),
            ChildNotebooks = new List<Notebook>(),
            FolderId = Guid.NewGuid(),
            FolderName = "~",
            OwnerId = newUser.Id
        };
        newUser.RootDir = rootDir;
    
        _dbContext.Folders.Add(rootDir);
        _dbContext.Users.Add(newUser);
        _dbContext.SaveChanges();

        var tokenString = GenerateToken(new UserLoginModel(model.Email, model.Password), newUser.Id);

        return Ok(new UserData(newUser.Id.ToString(),string.Empty,string.Empty, tokenString, string.Empty));
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

        var tokenString = GenerateToken(model, user.Id);

        return Ok(new UserData(user.Id.ToString(),string.Empty, string.Empty, tokenString, string.Empty));
    }
    
    private string GenerateToken(UserLoginModel loginUser,Guid id)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new(JwtRegisteredClaimNames.NameId, id.ToString())
            }),
            Expires = DateTime.UtcNow.Add(_tokenLifetime),
            Issuer = "http://75.119.133.95:5000",
            Audience = "http://75.119.133.95:5000",
            SigningCredentials = new SigningCredentials
            (
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TokenSecret)),
                SecurityAlgorithms.HmacSha256Signature
            )
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private readonly TimeSpan _tokenLifetime = new(0, 1, 0, 0);

    [GeneratedRegex(@"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")]
    private static partial Regex MyRegex();
    
    public static Guid? GetGuidFromToken(HttpContext httpContext)
    {
        var identity = httpContext.User.Identity as ClaimsIdentity;
        var claim = identity?.Claims.ToList().Find(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
        
        if(claim == null) return null;
        return new Guid(claim.Value);
    }
}