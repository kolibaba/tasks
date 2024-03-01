using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

var adminRole = new Role("admin");
var userRole = new Role("user");
var people = new List<Person>
{
    new("tom@gmail.com", "12345", adminRole),
    new("bob@gmail.com", "55555", userRole)
};

var builder = WebApplication.CreateBuilder();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/accessdenied";
    });
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization(); // добавление middleware авторизации 

app.MapGet("/accessdenied", async context =>
{
    context.Response.StatusCode = 403;
    await context.Response.WriteAsync("Access Denied");
});
app.MapGet("/login", async context =>
{
    context.Response.ContentType = "text/html; charset=utf-8";
    // html-форма для ввода логина/пароля
    var loginForm = @"<!DOCTYPE html>
    <html>
    <head>
        <meta charset='utf-8' />
        <title>METANIT.COM</title>
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
    await context.Response.WriteAsync(loginForm);
});

app.MapPost("/login", async (string? returnUrl, HttpContext context) =>
{
    var form = context.Request.Form;
    if (!form.ContainsKey("email") || !form.ContainsKey("password"))
    {
        return Results.BadRequest("Email и/или пароль не установлены");
    }
    string email = form["email"];
    string password = form["password"];

    var person = people.FirstOrDefault(p => p.Email == email && p.Password == password);
    if (person is null)
    {
        return Results.Unauthorized();
    }
    
    var claims = new List<Claim>
    {
        new(ClaimsIdentity.DefaultNameClaimType, person.Email),
        new(ClaimsIdentity.DefaultRoleClaimType, person.Role.Name)
    };
    var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
    await context.SignInAsync(claimsPrincipal);
    return Results.Redirect(returnUrl ?? "/");
});

app.Map("/admin", [Authorize(Roles = "admin")]() => "Admin Panel");

app.Map("/", [Authorize(Roles = "admin, user")](HttpContext context) =>
{
    var login = context.User.FindFirst(ClaimsIdentity.DefaultNameClaimType);
    var role = context.User.FindFirst(ClaimsIdentity.DefaultRoleClaimType);
    return $"Name: {login?.Value}\nRole: {role?.Value}";
});
app.MapGet("/logout", async (HttpContext context) =>
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return "Данные удалены";
});

app.Run();