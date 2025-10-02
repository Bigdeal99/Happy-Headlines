using DraftService.Data;
using DraftService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace DraftService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DraftsController : ControllerBase
{
    private readonly DraftDbContext _db;

    public DraftsController(DraftDbContext db) => _db = db;

    [HttpPost]
    public async Task<IActionResult> Create(Draft draft)
    {
        Log.Information("Saving draft Title={Title}", draft.Title);
        _db.Drafts.Add(draft);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = draft.Id }, draft);
    }

    [HttpGet]
    public async Task<IActionResult> List() =>
        Ok(await _db.Drafts.OrderByDescending(d => d.UpdatedAt).ToListAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var draft = await _db.Drafts.FindAsync(id);
        return draft is null ? NotFound() : Ok(draft);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Draft input)
    {
        var draft = await _db.Drafts.FindAsync(id);
        if (draft is null) return NotFound();

        draft.Title = input.Title;
        draft.Content = input.Content;
        draft.Author = input.Author;
        draft.Status = input.Status;
        await _db.SaveChangesAsync();

        Log.Information("Updated draft Id={Id}", id);
        return Ok(draft);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var draft = await _db.Drafts.FindAsync(id);
        if (draft is null) return NotFound();
        _db.Drafts.Remove(draft);
        await _db.SaveChangesAsync();
        Log.Information("Deleted draft Id={Id}", id);
        return NoContent();
    }
}
