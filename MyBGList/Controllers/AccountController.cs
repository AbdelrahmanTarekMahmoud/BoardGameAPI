using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MyBGList.Controllers
{
    [Route("[controller]/[Action]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApiUser> _userManager;
        private readonly SignInManager<ApiUser> _signInManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(ApplicationDbContext context, IConfiguration configuration, UserManager<ApiUser> userManager, SignInManager<ApiUser> signInManager, ILogger<AccountController> logger)
        {
            _context = context;
            _configuration = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [HttpPost]
        [ResponseCache(CacheProfileName = "NoCache")]
        public async Task<ActionResult> Register([FromBody] RegisterDTO request)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    var newUser = new ApiUser();
                    newUser.UserName = request.UserName;
                    newUser.Email = request.Email;

                    var result = await _userManager.CreateAsync(newUser, request.Password);
                    if(result.Succeeded)
                    {
                        _logger.LogInformation("User {userName} ({email} has been created."
                            , request.UserName, request.Email);
                        return StatusCode(201 , $"User '{newUser.UserName}' has been created.");
                    }
                    else
                    {
                        throw new Exception(string.Format("Error : {0}"
                            , string.Join(" ", result.Errors.Select(e => e.Description)))); // JUMPS TO CATCH
                    }
                }
                else
                {
                    var details = new ValidationProblemDetails(ModelState);
                    details.Type =
                    "https://tools.ietf.org/html/rfc7231#section-6.5.1";
                    details.Status = StatusCodes.Status400BadRequest;
                    return new BadRequestObjectResult(details);
                }
            }
            catch (Exception e) 
            {
                var exceptionDetails = new ProblemDetails();
                exceptionDetails.Detail = e.Message;
                exceptionDetails.Status =
                StatusCodes.Status500InternalServerError;
                exceptionDetails.Type =
                "https://tools.ietf.org/html/rfc7231#section-6.6.1";
                return StatusCode(
                StatusCodes.Status500InternalServerError,
                exceptionDetails);
            }
        }
        [HttpPost]
        [ResponseCache(CacheProfileName = "NoCache")]
        public async Task<ActionResult> Login([FromBody] LoginDTO request)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    var user = await _userManager.FindByNameAsync(request.UserName);
                    if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
                        throw new Exception("Invalid login attempt.");
                    else
                    {
                        //Generates the signing credentials
                        var signingCredentials = new SigningCredentials( 
                            new SymmetricSecurityKey(
                                System.Text.Encoding.UTF8.GetBytes(_configuration["JWT:SigningKey"])),
                                SecurityAlgorithms.HmacSha256
                        );

                        //Sets up the user claims
                        var claims = new List<Claim>(); 
                        claims.Add(new Claim(ClaimTypes.Name, user.UserName));
                        claims.AddRange((await _userManager.GetRolesAsync(user)).Select(r => new Claim(ClaimTypes.Role, r)));

                        //Instantiates a JWT object instance
                        var jwtObject = new JwtSecurityToken( 
                            issuer: _configuration["JWT:Issuer"],
                            audience: _configuration["JWT:Audience"],
                            claims: claims,
                            expires: DateTime.Now.AddSeconds(300),
                            signingCredentials: signingCredentials
                        );
                        //Generates the JWT encrypted string
                        var jwtString = new JwtSecurityTokenHandler().WriteToken(jwtObject);

                        return StatusCode(StatusCodes.Status200OK, jwtString);
                    }
                }
                else
                {
                    var details = new ValidationProblemDetails(ModelState);
                    details.Type =
                    "https://tools.ietf.org/html/rfc7231#section-6.5.1";
                    details.Status = StatusCodes.Status400BadRequest;
                    return new BadRequestObjectResult(details);
                }
            }
            catch(Exception e)
            {
                var exceptionDetails = new ProblemDetails();
                exceptionDetails.Detail = e.Message;
                exceptionDetails.Status =
                StatusCodes.Status500InternalServerError;
                exceptionDetails.Type =
                "https://tools.ietf.org/html/rfc7231#section-6.6.1";
                return StatusCode(
                StatusCodes.Status500InternalServerError,
                exceptionDetails);
            }
        }

    }
}
