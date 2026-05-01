using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Taskify.Api.Dtos;
using Taskify.Database;

namespace Taskify.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class UsersController : ControllerBase
{
    private readonly TaskifyDbContext context;

    public UsersController(TaskifyDbContext context)
    {
        this.context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<UserDto>>> GetUsers()
    {
        var users = await context.Users
            .OrderBy(user => user.Id)
            .ToListAsync();

        return users.Select(user => user.ToDto()).ToList();
    }
}
