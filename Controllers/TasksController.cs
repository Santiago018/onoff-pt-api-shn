using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnOff.Api.Application.DTOs;
using OnOff.Api.Domain.Entities;
using OnOff.Api.Infrastructure.Data;

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
            var query = _context.Tasks.AsQueryable();

            if (completed.HasValue)
                query = query.Where(t => t.IsCompleted == completed.Value);

            var tasks = query
                .OrderBy(t => t.IsCompleted)
                .ThenByDescending(t => t.CreatedAt)
                .ToList();

            return Ok(tasks);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateTaskDto dto)
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

            return Ok(task);
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, [FromBody] UpdateTaskDto dto)
        {
            var task = _context.Tasks.FirstOrDefault(t => t.Id == id);

            if (task == null)
                return NotFound();

            task.Title = dto.Title;
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPut("{id}/status")]
        public IActionResult ChangeStatus(Guid id, [FromBody] UpdateTaskStatusDto dto)
        {
            var task = _context.Tasks.FirstOrDefault(t => t.Id == id);

            if (task == null)
                return NotFound();

            task.IsCompleted = dto.IsCompleted;
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var task = _context.Tasks.FirstOrDefault(t => t.Id == id);

            if (task == null)
                return NotFound();

            _context.Tasks.Remove(task);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
