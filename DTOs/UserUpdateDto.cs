using System.ComponentModel.DataAnnotations;

namespace UserManagementAPI.DTOs
{
    public class UserUpdateDto
    {
        [MaxLength(50)]
        public string? FirstName { get; set; }

        [MaxLength(50)]
        public string? LastName { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [MaxLength(100)]
        public string? Department { get; set; }

        public bool? IsActive { get; set; }
    }
}
