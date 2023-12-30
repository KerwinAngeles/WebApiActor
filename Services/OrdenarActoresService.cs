using WebApiActor.Models;
using WebApiActor.Services.Interfaces;

namespace WebApiActor.Services
{
    public class OrdenarActoresService : IOrdenarActores
    {
       public void OrdenarActores(Pelicula pelicula)
        {
            if (pelicula.ActoresPeliculas != null)
            {
                for (int i = 1; i < pelicula.ActoresPeliculas.Count; i++)
                {
                    pelicula.ActoresPeliculas[i].Orden = i;
                }
            }
        }
    }
}
