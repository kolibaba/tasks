using System.Security.Claims;
using AuthService.Users;
using Common;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder();

builder.AddAuthService();

builder.Services.AddControllers();

var app = builder.Build();

app.UseRouting();
app.UseCors();
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuth();

app.MapControllerRoute(
    "default",
    "{controller=Home}/{action=Index}/{id?}");

await UserService.Instance.Init();

app.Map("/data", [Authorize]() => new { message = "Hello World!" });

app.MapGet("/logout", async (HttpContext context) =>
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return "Данные удалены";
});

// доступ только для ролей admin и user
app.Map("/info", [Authorize(Roles = "admin, user")](HttpContext context) =>
{
    var login = context.User.FindFirst(ClaimsIdentity.DefaultNameClaimType);
    var role = context.User.FindFirst(ClaimsIdentity.DefaultRoleClaimType);
    return $"Name: {login?.Value}\nRole: {role?.Value}";
});

app.Run();