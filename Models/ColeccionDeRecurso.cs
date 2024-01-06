namespace WebApiActor.Models
{
    public class ColeccionDeRecurso<T> : Recurso where T : Recurso
    {
        public List<T> Valores { get; set; }
    }
}
