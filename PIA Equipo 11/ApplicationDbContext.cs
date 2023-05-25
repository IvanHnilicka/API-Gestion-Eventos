using Microsoft.EntityFrameworkCore;
using PIA_Equipo_11.Entidades;

namespace PIA_Equipo_11

{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base (options){            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            

            modelBuilder.Entity<ComentariosUsuario>(entity => {
                // Se establece la llave primaria de la tabla ComentariosUSuario
                //Como una llave compuesta por dos claves foráneas
                //El orden debe ser el mismo que el declarado en las entidades
                entity.HasKey(cu => new { cu.UsuarioId, cu.EventoId });
                entity.HasOne(cu => cu.Usuario)
                .WithMany(u => u.ComentariosUsuario)
                .HasForeignKey(cu => cu.UsuarioId).OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(cu => cu.Evento)
                .WithMany(e => e.ComentariosUsuario)
                .HasForeignKey(cu => cu.EventoId).OnDelete(DeleteBehavior.Restrict);

            });

            modelBuilder.Entity<EventosFavoritos>(entity =>
            {
                entity.HasKey(ef => new { ef.UsuarioId, ef.EventoId });
                entity.HasOne(ef => ef.Usuario)
                    .WithMany(u => u.EventosFavoritos)
                    .HasForeignKey(ef => ef.UsuarioId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(ef => ef.Evento)
                .WithMany(e => e.EventosFavoritos)
                .HasForeignKey(ef => ef.EventoId).OnDelete(DeleteBehavior.Restrict);
            });



            modelBuilder.Entity<RegistroEventos>(entity =>
            {
                entity.HasKey(re => new { re.UsuarioId, re.EventoId });
                entity.HasOne(re => re.Usuario)
                .WithMany(u => u.RegistroEventos )
                .HasForeignKey(re => re.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(re => re.Evento)
                .WithMany(e => e.RegistroEventos)
                .HasForeignKey(re => re.EventoId)
                .OnDelete(DeleteBehavior.Restrict);

            });
        }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Evento> Eventos { get; set; }
        public DbSet<Codigo> Codigos { get; set; }
        public DbSet<EventosFavoritos> EventosFavoritos { get; set; }
        public DbSet<RegistroEventos> RegistroEventos { get; set; }
        public DbSet<ComentariosUsuario> ComentariosUsuarios { get; set; }

    }

}
