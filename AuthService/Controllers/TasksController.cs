using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tasksManager.Tasks;

namespace tasksManager.Controllers;

[Route("tasks")]
public class TasksController : Controller
{
    private readonly TasksService _tasksService;

    //TODO: fix DInjection later
    /*public TasksController(TasksService tasksService, UsersRepository usersRepository)
    {
        _tasksService = tasksService;
        _usersRepository = usersRepository;
    }*/

    public TasksController( /*TasksService tasksService, UsersRepository usersRepository*/)
    {
        _tasksService = TasksService.Instance;
    }

    [HttpGet("create")]
    [Authorize(Roles = "admin, user")]
    public async Task Index()
    {
        var login = HttpContext.User.FindFirst(ClaimsIdentity.DefaultNameClaimType);
        HttpContext.Response.ContentType = "text/html; charset=utf-8";
        var loginForm = $@"<!DOCTYPE html>
    <html>
    <head>
        <meta charset='utf-8' />
        <title>create task for {login}</title>
    </head>
    <body>
        <h2>Create task form</h2>
        <form method='post'>
            <p>
                <label>Text</label><br />
                <input name='text' />
            </p>            
            <input type='submit' value='Create' />
        </form>
    </body>
    </html>";
        await HttpContext.Response.WriteAsync(loginForm);
    }

    [HttpPost("create")]
    [Authorize(Roles = "admin, user")]
    public IActionResult Create()
    {
        var form = HttpContext.Request.Form;
        if (!form.ContainsKey("text"))
            return BadRequest("text is empty");

        var text = form["text"].ToString();

        if (string.IsNullOrEmpty(text))
            return BadRequest("text is empty");

        _tasksService.Create(text);

        return Content("task created");
    }
}