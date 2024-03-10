using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AuthService.Users;
using Common;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Controllers;

public record class Person(string Email, string Password);

public class HomeController : Controller
{
    [HttpGet("login")]
    public async Task Index(string? returnUrl)
    {
        HttpContext.Response.ContentType = "text/html; charset=utf-8";
        var loginForm = @"<!DOCTYPE html>
    <html>
    <head>
        <meta charset='utf-8' />
        <title>Login</title>
    </head>
    <body>
        <h2>Login Form</h2>
        <form method='post'>
            <p>
                <label>Email</label><br />
                <input name='email' />
            </p>
            <p>
                <label>Password</label><br />
                <input type='password' name='password' />
            </p>
            <input type='submit' value='Login' />
        </form>
    </body>
    </html>";
        await HttpContext.Response.WriteAsync(loginForm);
    }

    [HttpPost("login")]
    public IActionResult Login(string? returnUrl, [FromForm] Person loginData)
    {
        // находим пользователя 
        var user = UsersRepository.Instance.GetAll()
            .FirstOrDefault(p => p.Email == loginData.Email && p.Password == loginData.Password);
        // если пользователь не найден, отправляем статусный код 401
        if (user is null) return Unauthorized();

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.Email),
            new(ClaimTypes.Role, user.Role.Name)
        };
        // создаем JWT-токен
        var jwt = new JwtSecurityToken(
            AuthOptions.ISSUER,
            AuthOptions.AUDIENCE,
            claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(2)),
            signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(),
                SecurityAlgorithms.HmacSha256));
        var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

        HttpContext.Response.Cookies.Append(".AspNetCore.Application.Id", encodedJwt,
            new CookieOptions
            {
                MaxAge = TimeSpan.FromHours(60)
            });
        return Redirect(returnUrl ?? "/info");
    }

    [HttpGet("register")]
    public async Task Register()
    {
        HttpContext.Response.ContentType = "text/html; charset=utf-8";
        var loginForm = @"<!DOCTYPE html>
    <html>
    <head>
        <meta charset='utf-8' />
        <title>Register new user</title>
    </head>
    <body>
        <h2>Login Form</h2>
        <form method='post'>
            <p>
                <label>Email</label><br />
                <input name='email' />
            </p>
            <p>
                <label>Password</label><br />
                <input type='password' name='password' />
            </p>
            <p>
                <label>Role</label><br />
                <input name='role' value='user'/>
            </p>
            <input type='submit' value='Register' />
        </form>
    </body>
    </html>";
        await HttpContext.Response.WriteAsync(loginForm);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(string? returnUrl)
    {
        var form = HttpContext.Request.Form;
        if (!form.ContainsKey("email") || !form.ContainsKey("password"))
            return BadRequest("Email и/или пароль не установлены");

        string email = form["email"];
        string password = form["password"];
        string role = form["role"];

        var user = UsersRepository.Instance.GetAll().FirstOrDefault(p => p.Email == email);
        if (user is not null) return BadRequest("Уже есть такой юзер");

        user = await UserService.Instance.Create(email, password, role);

        var claims = new List<Claim>
        {
            new(ClaimsIdentity.DefaultNameClaimType, user.Email),
            new(ClaimsIdentity.DefaultRoleClaimType, user.Role.Name)
        };
        var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        await HttpContext.SignInAsync(claimsPrincipal);

        return Redirect(returnUrl ?? "/");
    }
}