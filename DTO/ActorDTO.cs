using System.ComponentModel.DataAnnotations;

namespace WebApiActor.DTO
{
    public class ActorDTO
    {
        [Required]
        [StringLength(maximumLength: 120)]
        public string Name {  get; set; }
        public string LastName { get; set; }
        public int age { get; set; }
    }
}
