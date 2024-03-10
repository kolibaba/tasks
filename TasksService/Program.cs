using Common;
using tasksManager.Tasks;

var builder = WebApplication.CreateBuilder();

builder.AddAuthService();

//builder.Services.AddMvc(options => options.EnableEndpointRouting = false);
builder.Services.AddControllers();
var app = builder.Build();

//app.UseMvc();
app.UseRouting();
app.UseCors();
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuth();

app.MapControllerRoute(
    "default",
    "{controller=Home}/{action=Index}/{id?}");

TasksService.Instance.Init();

app.Run();