using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.Dtos
{
    public class UserForRegisterDto
    {
        [Required]
        [StringLength(18,MinimumLength = 5, ErrorMessage = "Username must be between 5 and 18 characters")]
        public string Username { get; set; }
        

        [Required]
        [StringLength(16,MinimumLength = 6, ErrorMessage = "Password Length must be between 6 and 16 characters")]
        public string Password { get; set; }
    }
}