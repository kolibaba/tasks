using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using tasksManager.Tasks;
using tasksManager.Users;

var builder = WebApplication.CreateBuilder();

builder.Services.AddAuthorization();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/accessdenied";
    });
builder.Services.AddControllers();
var app = builder.Build();

app.UseRouting();
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.UseStatusCodePages(context =>
{
    var response = context.HttpContext.Response;

    if (response.StatusCode == (int)HttpStatusCode.Unauthorized ||
        response.StatusCode == (int)HttpStatusCode.Forbidden)
        response.Redirect("/");
    return Task.CompletedTask;
});

app.MapControllerRoute(
    "default",
    "{controller=Home}/{action=Index}/{id?}");

app.Map("/admin", [Authorize(Roles = "admin")]() => { return "Admin Panel"; });

app.MapGet("/accessdenied", async context =>
{
    context.Response.StatusCode = 403;
    await context.Response.WriteAsync("Access Denied");
});

//TODO: где и как лучше запускать подписки на события?
TasksService.Instance.Init();
await UserService.Instance.Init();


//app.Map("/admin", [Authorize(Roles = "admin")]() => "Admin Panel");

// доступ только для ролей admin и user
/*app.Map("/", [Authorize(Roles = "admin, user")](HttpContext context) =>
{
    var login = context.User.FindFirst(ClaimsIdentity.DefaultNameClaimType);
    var role = context.User.FindFirst(ClaimsIdentity.DefaultRoleClaimType);
    return $"Name: {login?.Value}\nRole: {role?.Value}";
});*/
app.MapGet("/logout", async (HttpContext context) =>
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return "Данные удалены";
});

app.Run();