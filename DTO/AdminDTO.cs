using System.ComponentModel.DataAnnotations;

namespace WebApiActor.DTO
{
    public class AdminDTO
    {
        [Required]
        [EmailAddress]
        public string email { get; set; }
    }
}
