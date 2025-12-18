using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnOff.Api.Application.DTOs;
using OnOff.Api.Domain.Entities;
using OnOff.Api.Infrastructure.Data;
using System.Security.Claims;

namespace OnOff.Api.Controllers
{
    [ApiController]
    [Route("api/tasks")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TasksController(AppDbContext context)
        {
            _context = context;
        }


        private Guid GetUserId()
        {
            var userIdClaim = User.Claims.First(c => c.Type == "userId").Value;
            return Guid.Parse(userIdClaim);
        }


        [HttpGet]
        public IActionResult GetAll([FromQuery] bool? completed)
        {
            var userId = GetUserId();

            var query = _context.Tasks
                .Where(t => t.UserId == userId);

            if (completed.HasValue)
                query = query.Where(t => t.IsCompleted == completed.Value);

            var tasks = query
                .OrderBy(t => t.IsCompleted)
                .ThenByDescending(t => t.CreatedAt)
                .ToList();

            return Ok(tasks);
        }


        [HttpPost]
        public IActionResult Create(CreateTaskDto dto)
        {
            var userId = GetUserId();

            var task = new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                IsCompleted = false,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Tasks.Add(task);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetAll), new { id = task.Id }, task);
        }


        [HttpPut("{id}")]
        public IActionResult Update(Guid id, UpdateTaskDto dto)
        {
            var userId = GetUserId();

            var task = _context.Tasks
                .FirstOrDefault(t => t.Id == id && t.UserId == userId);

            if (task == null)
                return NotFound();

            task.Title = dto.Title;
            _context.SaveChanges();

            return NoContent();
        }


        [HttpPut("{id}/status")]
        public IActionResult ChangeStatus(Guid id, UpdateTaskStatusDto dto)
        {
            var userId = GetUserId();

            var task = _context.Tasks
                .FirstOrDefault(t => t.Id == id && t.UserId == userId);

            if (task == null)
                return NotFound();

            task.IsCompleted = dto.IsCompleted;
            _context.SaveChanges();

            return NoContent();
        }


        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var userId = GetUserId();

            var task = _context.Tasks
                .FirstOrDefault(t => t.Id == id && t.UserId == userId);

            if (task == null)
                return NotFound();

            _context.Tasks.Remove(task);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
