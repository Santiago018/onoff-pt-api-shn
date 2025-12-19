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

        // =========================
        // GET ALL (abierto)
        // =========================
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

        // =========================
        // CREATE (necesita UserId por FK)
        // =========================
        [HttpPost]
        public IActionResult Create([FromBody] CreateTaskDto dto)
        {
            var userId = GetUserId(); // ✅ para cumplir FK

            var task = new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                IsCompleted = false,
                UserId = userId,          // ✅ CLAVE
                CreatedAt = DateTime.UtcNow
            };

            _context.Tasks.Add(task);
            _context.SaveChanges();

            return Ok(task);
        }

        // =========================
        // UPDATE TITLE (sin filtrar por UserId)
        // =========================
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

        // =========================
        // CHANGE STATUS (sin filtrar por UserId)
        // =========================
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

        // =========================
        // DELETE (sin filtrar por UserId)
        // =========================
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
