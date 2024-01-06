using WebApiActor.Models;

namespace WebApiActor.DTO
{
    public class ActorDTOId : Recurso
    {
        public int Id { get; set; } 
        public string Name { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
    }
}
