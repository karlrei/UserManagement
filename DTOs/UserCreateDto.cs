using System.ComponentModel.DataAnnotations;

namespace UserManagementAPI.DTOs
{
    public class UserCreateDto
    {
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = default!;

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = default!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = default!;

        [MaxLength(100)]
        public string? Department { get; set; }
    }
}
