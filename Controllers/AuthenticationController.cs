using Microsoft.AspNetCore.Mvc;
using PersonalFinanceTracker.Models.DTO;
using PersonalFinanceTracker.Services.Authentication;
using PersonalFinanceTracker.Services.JWT;

namespace PersonalFinanceTracker.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IJwtService _jwtService;

    public AuthenticationController(IAuthenticationService authenticationService, IJwtService jwtService)
    {
        _authenticationService  = authenticationService;
        _jwtService = jwtService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Registration(RegisterDto registerDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        try
        {
            var user = await _authenticationService.RegisterUserAsync(registerDto);
        
            var token = _jwtService.GenerateToken(user);
        
            var responseDto = new AuthResponseDto
            {
                Email = user.Email, Token = token, ExpiresAt = _jwtService.GetTokenExpiration()
            };
        
            return Created("", responseDto);
        }
        catch (InvalidOperationException e)
        {
            return Conflict("User with this email already exists");
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _authenticationService.ValidateUserCredentialsAsync(loginDto);

        if (user is null)
        {
            return Unauthorized("Invalid email or password");
        }
        
        var token = _jwtService.GenerateToken(user);
        
        var responseDto = new AuthResponseDto
        {
            Email = user.Email, Token = token, ExpiresAt = _jwtService.GetTokenExpiration()
        };
        
        return Ok(responseDto);
    }
}