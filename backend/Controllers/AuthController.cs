using DataAccess.Repository;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Auth;

namespace Controllers;

[Route("[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    readonly IUnitOfWork _unit;
    readonly ITokenService _tokenService;
    public AuthController(IUnitOfWork unit, ITokenService tokenService)
    {
        _unit = unit;
        _tokenService = tokenService;
    }

    [HttpPost("Registrate")]
    public IActionResult Registrate([FromBody] RegistrationRequest request)
    {
        var user = _unit.UserRepo.First(u => u.Login == request.Login);
        if (user is not null) return BadRequest("Login already taken");

        IEnumerable<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, request.Login),
                new Claim(ClaimTypes.Role, Roles.Default)
            };

        var accessToken = _tokenService.GenerateAccessToken(claims);
        var refreshToken = _tokenService.GenerateRefreshToken();

        _unit.UserRepo.Add(new User()
        {
            Login = request.Login,
            DisplayName = request.DisplayName,
            Password = new PasswordHasher<string>()
                .HashPassword(request.Login, request.Password),
            School = request.School,
            RefreshToken = refreshToken,
            RefreshTokenExp = DateTime.Now.AddDays(14)
        });

        _unit.Save();

        return Ok(new
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        });
    }

    [HttpPost("Login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var user = _unit.UserRepo.First(u => u.Login == request.Login);
        if (user is null) return BadRequest("User not found");

        if (new PasswordHasher<string>().VerifyHashedPassword(user.Login, user.Password, request.Password) == PasswordVerificationResult.Failed)
            return BadRequest("Password invalid");

        IEnumerable<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.Login),
                new Claim(ClaimTypes.Role, user.Role)
            };

        var accessToken = _tokenService.GenerateAccessToken(claims);
        var refreshToken = _tokenService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExp = DateTime.Now.AddDays(14);

        _unit.UserRepo.Update(user);
        _unit.Save();

        return Ok(new
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        });
    }

    [HttpPost("Refresh")]
    public IActionResult Refresh([FromBody] RefreshRequest request)
    {
        var principal = _tokenService.GetPrincipalFromExpiredToken(request.AccessToken);
        var userLogin = principal.Identity?.Name;
        if (userLogin is null) return BadRequest("Token invalid");

        var user = _unit.UserRepo.First(u => u.Login == userLogin);
        if (user is null) return BadRequest("User not found");

        if (request.RefreshToken != user.RefreshToken || user.RefreshTokenExp < DateTime.Now)
            return BadRequest("Refresh token invalid");

        var newAccessToken = _tokenService.GenerateAccessToken(principal.Claims);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExp = DateTime.Now.AddDays(14);

        _unit.UserRepo.Update(user);
        _unit.Save();

        return Ok(new
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        });
    }

    [AuthFilter("default")]
    [HttpPost("Revoke")]
    public IActionResult Revoke()
    {
        var userName = User.Identity?.Name;

        var user = _unit.UserRepo.First(user => user.Login == userName);
        if (user is null) return BadRequest();

        user.RefreshToken = null;
        user.RefreshTokenExp = null;

        _unit.UserRepo.Update(user);
        _unit.Save();

        return Ok();
    }
}