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
public class ProjectsController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public ProjectsController(ApplicationDbContext db) => _db = db;

    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Project>>> GetMyProjects()
    {
        var userId = UserHelper.GetUserId(User);

        var projects = await _db.Projects
            .Include(p => p.Tasks)
            .Where(p => p.OwnerId == userId || p.Tasks.Any(t => t.AssignedUserId == userId))
            .ToListAsync();

        return Ok(projects);
    }

    
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Project>> GetProject(int id)
    {
        var userId = UserHelper.GetUserId(User);

        var project = await _db.Projects
            .Include(p => p.Tasks)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (project == null) return NotFound();

        var canAccess = project.OwnerId == userId ||
                        project.Tasks.Any(t => t.AssignedUserId == userId);

        if (!canAccess) return Forbid();

        return Ok(project);
    }

    
    [HttpPost]
    public async Task<ActionResult<Project>> CreateProject(ProjectCreateDto dto)
    {
        var userId = UserHelper.GetUserId(User);

        var project = new Project
        {
            Name = dto.Name,
            Description = dto.Description,
            OwnerId = userId
        };

        _db.Projects.Add(project);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetProject), new { id = project.Id }, project);
    }

    
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateProject(int id, ProjectUpdateDto dto)
    {
        var userId = UserHelper.GetUserId(User);

        var project = await _db.Projects.FirstOrDefaultAsync(p => p.Id == id);
        if (project == null) return NotFound();

        if (project.OwnerId != userId) return Forbid();

        project.Name = dto.Name;
        project.Description = dto.Description;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        var userId = UserHelper.GetUserId(User);

        var project = await _db.Projects.FirstOrDefaultAsync(p => p.Id == id);
        if (project == null) return NotFound();

        if (project.OwnerId != userId) return Forbid();

        _db.Projects.Remove(project);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
