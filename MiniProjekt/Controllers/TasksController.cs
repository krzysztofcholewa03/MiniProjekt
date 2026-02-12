using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniProjekt.Data;
using MiniProjekt.Dtos;
using MiniProjekt.Helpers;
using MiniProjekt.Models;

namespace MiniProjekt.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public TasksController(ApplicationDbContext db) => _db = db;

    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskItem>>> GetMyTasks()
    {
        var userId = UserHelper.GetUserId(User);

        var tasks = await _db.TaskItems
            .Include(t => t.Project)
            .Where(t => t.Project.OwnerId == userId || t.AssignedUserId == userId)
            .ToListAsync();

        return Ok(tasks);
    }

    
    [HttpGet("{id:int}")]
    public async Task<ActionResult<TaskItem>> GetTask(int id)
    {
        var userId = UserHelper.GetUserId(User);

        var task = await _db.TaskItems
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (task == null) return NotFound();

        var canAccess = task.Project.OwnerId == userId || task.AssignedUserId == userId;
        if (!canAccess) return Forbid();

        return Ok(task);
    }

    
    [HttpPost]
    public async Task<ActionResult<TaskItem>> CreateTask(TaskCreateDto dto)
    {
        var userId = UserHelper.GetUserId(User);

        var project = await _db.Projects.FirstOrDefaultAsync(p => p.Id == dto.ProjectId);
        if (project == null) return BadRequest("ProjectId nie istnieje.");

        if (project.OwnerId != userId) return Forbid();

        var task = new TaskItem
        {
            Title = dto.Title,
            Description = dto.Description,
            ProjectId = dto.ProjectId,
            AssignedUserId = dto.AssignedUserId
        };

        _db.TaskItems.Add(task);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
    }

    
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateTask(int id, TaskUpdateDto dto)
    {
        var userId = UserHelper.GetUserId(User);

        var task = await _db.TaskItems
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (task == null) return NotFound();

        var canEdit = task.Project.OwnerId == userId || task.AssignedUserId == userId;
        if (!canEdit) return Forbid();

        
        if (dto.ProjectId != task.ProjectId)
        {
            var newProject = await _db.Projects.FirstOrDefaultAsync(p => p.Id == dto.ProjectId);
            if (newProject == null) return BadRequest("Nowy ProjectId nie istnieje.");
            if (newProject.OwnerId != userId) return Forbid();

            task.ProjectId = dto.ProjectId;
        }

        task.Title = dto.Title;
        task.Description = dto.Description;
        task.IsDone = dto.IsDone;

        
        if (task.Project.OwnerId == userId)
            task.AssignedUserId = dto.AssignedUserId;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        var userId = UserHelper.GetUserId(User);

        var task = await _db.TaskItems
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (task == null) return NotFound();
        if (task.Project.OwnerId != userId) return Forbid();

        _db.TaskItems.Remove(task);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
