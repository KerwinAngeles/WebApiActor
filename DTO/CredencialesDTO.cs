using System.ComponentModel.DataAnnotations;

namespace WebApiActor.DTO
{
    public class CredencialesDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
