using System.ComponentModel.DataAnnotations;

namespace PropertyManagementSystem.Dtos
{
    public class RegisterRequestDto
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

       
        public string Role { get; set; }

        public string CompanyName { get; set; }

        [Required]
        public string Address { get; set; }

    }
}
