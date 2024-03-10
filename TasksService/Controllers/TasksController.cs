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

    [HttpGet("all")]
    //[Authorize(Roles = "admin, user")]
    public async Task Index()
    {
        var list = _tasksService.GetAllTasks();
        var rows = "";
        list.ForEach(x => rows += $@"
<tr>
    <td>{x.Id}</td>
    <td>{x.Text}</td>
    <td>{x.IsCompleted.ToString()}</td>
</tr>");
        //return View("Index", list);
        var loginForm = $@"<!DOCTYPE html>
    <html>
    <head>
        <meta charset='utf-8' />
        <title>All tasks</title>
    </head>
    <body>
        <h2>All tasks</h2>
<table>
        <tr>
            <th>Id</th>
            <th>Text</th>
            <th>IsCompleted</th>
        </tr>
           {rows}
<table>
    </body>
    </html>";
        await HttpContext.Response.WriteAsync(loginForm);
    }

    [HttpGet("create")]
    [Authorize(Roles = "admin, user")]
    public async Task OnCreate()
    {
        var login = HttpContext.User.FindFirst(ClaimsIdentity.DefaultNameClaimType)?.Value;
        HttpContext.Response.ContentType = "text/html; charset=utf-8";
        var loginForm = $@"<!DOCTYPE html>
    <html>
    <head>
        <meta charset='utf-8' />
        <title>create new task</title>
    </head>
    <body>
        <h2>Create task (user: {login})</h2>
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
    //[Authorize(Roles = "admin, user")]
    public IActionResult Create()
    {
        var user = HttpContext.User;
        var form = HttpContext.Request.Form;
        if (!form.ContainsKey("text"))
            return BadRequest("text is empty");

        var text = form["text"].ToString();

        if (string.IsNullOrEmpty(text))
            return BadRequest("text is empty");

        var taskItem = _tasksService.Create(text);

        //return Json(taskItem);
        return Redirect("all");
    }

    [HttpPost("complete")]
    [Authorize(Roles = "admin, user")]
    public async Task<IActionResult> Complete(Guid taskId)
    {
        //var login = HttpContext.User.FindFirst(ClaimsIdentity.);
        var currentUserId = Guid.Empty; //TODO detect user
        await _tasksService.Complete(taskId, currentUserId);
        return Ok();
    }

    [HttpPost("shuffle")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Shuffle()
    {
        await _tasksService.Shuffle();
        return Ok();
    }
}