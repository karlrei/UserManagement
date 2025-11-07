using Microsoft.AspNetCore.Mvc;
using UserManagementAPI.DTOs;
using UserManagementAPI.Models;
using UserManagementAPI.Repositories;

namespace UserManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _repo;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserRepository repo, ILogger<UsersController> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 25, [FromQuery] string? department = null, CancellationToken ct = default)
        {
            pageNumber = Math.Max(1, pageNumber);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var users = await _repo.GetAllAsync(ct);
            if (!string.IsNullOrWhiteSpace(department))
                users = users.Where(u => string.Equals(u.Department, department, StringComparison.OrdinalIgnoreCase));

            var page = users.Skip((pageNumber - 1) * pageSize).Take(pageSize)
                .Select(u => new UserReadDto
                {
                    Id = u.Id,
                    FullName = $"{u.FirstName} {u.LastName}",
                    Email = u.Email,
                    Department = u.Department,
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt
                });

            return Ok(new { pageNumber, pageSize, items = page });
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<UserReadDto>> GetById(Guid id)
        {
            var u = await _repo.GetByIdAsync(id);
            if (u == null) return NotFound();
            var dto = new UserReadDto
            {
                Id = u.Id,
                FullName = $"{u.FirstName} {u.LastName}",
                Email = u.Email,
                Department = u.Department,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt
            };
            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<UserReadDto>> Create([FromBody] UserCreateDto create)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existing = await _repo.GetByEmailAsync(create.Email);
            if (existing != null) return Conflict(new { error = "Email already in use." });

            var user = new User
            {
                FirstName = create.FirstName,
                LastName = create.LastName,
                Email = create.Email,
                Department = create.Department
            };

            var created = await _repo.CreateAsync(user);

            var dto = new UserReadDto
            {
                Id = created.Id,
                FullName = $"{created.FirstName} {created.LastName}",
                Email = created.Email,
                Department = created.Department,
                IsActive = created.IsActive,
                CreatedAt = created.CreatedAt
            };

            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UserUpdateDto update)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return NotFound();

            if (!string.IsNullOrWhiteSpace(update.FirstName)) existing.FirstName = update.FirstName;
            if (!string.IsNullOrWhiteSpace(update.LastName)) existing.LastName = update.LastName;
            if (!string.IsNullOrWhiteSpace(update.Email)) existing.Email = update.Email;
            if (update.Department != null) existing.Department = update.Department;
            if (update.IsActive.HasValue) existing.IsActive = update.IsActive.Value;

            var ok = await _repo.UpdateAsync(existing);
            if (!ok) return StatusCode(500, new { error = "Unable to update user." });
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return NotFound();

            var ok = await _repo.DeleteAsync(id);
            if (!ok) return StatusCode(500, new { error = "Unable to delete user." });
            return NoContent();
        }
    }
}
