using Atlas_Web.Contracts.Api.Tasks;
using Atlas_Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas_Web.Controllers.Api;

[ApiController]
[Route("api/tasks")]
[Authorize(AuthenticationSchemes = "Bearer")]
public class TasksApiController : ControllerBase
{
    private readonly ITasksApiService _tasksApiService;

    public TasksApiController(ITasksApiService tasksApiService)
    {
        _tasksApiService = tasksApiService;
    }

    [HttpGet]
    public async Task<ActionResult<TasksResponseDto>> GetTasks(
        CancellationToken cancellationToken = default
    )
    {
        return Ok(await _tasksApiService.GetTasksAsync(User, cancellationToken));
    }
}
