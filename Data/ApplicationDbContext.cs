using Microsoft.EntityFrameworkCore;
using WebApiActor.Models;

namespace WebApiActor.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) 
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ActorPelicula>().HasKey(ap => new {ap.ActorId, ap.PeliculaId});
        }

        public DbSet<Actor> Actors { get; set; }
        public DbSet<Pelicula> Peliculas { get; set;}
        public DbSet<Comentario> Comentarios { get; set; }
        public DbSet<ActorPelicula> ActorPeliculas { get; set; }
    }
}
