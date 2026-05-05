using Microsoft.EntityFrameworkCore;
using SCADA.Modelos;

namespace SCADA.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Maquina> Maquinas { get; set; }
        public DbSet<Empleado> Empleados { get; set; }
        public DbSet<MaquinaProduccion> Producciones { get; set; }
        public DbSet<MaquinaEstatus> Estatus { get; set; }
        public DbSet<Orden> Ordenes { get; set; }
        public DbSet<Seccion> Secciones { get; set; }
        public DbSet<ImputacionOperario> ImputacionesOperarios { get; set; }
        public DbSet<Operacion> Operaciones { get; set; }
        public DbSet<OperacionesOrden> OperacionesOrden { get; set; }
        public DbSet<MaquinaOperario> MaquinasOperarios { get; set; }
        public DbSet<Material> Materiales { get; set; }
        public DbSet<ImputacionMaterial> ImputacionMateriales { get; set; }
        public DbSet<MaquinaMaterial> MaquinasMateriales { get; set; }
        public DbSet<EmpleadoMaquina> EmpleadoMaquinas { get; set; }
        public DbSet<TipoOperacion> TiposOperaciones { get; set; }
        public DbSet<TipoOperacionMaterial> TiposOperacionesMateriales { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Mapeo de Tablas
            modelBuilder.Entity<Maquina>().ToTable("Maquina");
            modelBuilder.Entity<MaquinaProduccion>().ToTable("MaquinaProduccion");
            modelBuilder.Entity<MaquinaEstatus>().ToTable("MaquinaEstatus");
            modelBuilder.Entity<Orden>().ToTable("Ordenes");
            modelBuilder.Entity<Seccion>().ToTable("Secciones");
            modelBuilder.Entity<Empleado>().ToTable("Empleados");
            modelBuilder.Entity<ImputacionOperario>().ToTable("ImputacionOperarios");
            modelBuilder.Entity<Operacion>().ToTable("Operaciones");
            modelBuilder.Entity<OperacionesOrden>().ToTable("OperacionesOrden");
            modelBuilder.Entity<MaquinaOperario>().ToTable("MaquinasOperarios");
            modelBuilder.Entity<Material>().ToTable("Materiales");
            modelBuilder.Entity<ImputacionMaterial>().ToTable("ImputacionMateriales");
            modelBuilder.Entity<MaquinaMaterial>().ToTable("MaquinasMateriales");
            modelBuilder.Entity<EmpleadoMaquina>().ToTable("MaquinasEmpleados");
            modelBuilder.Entity<TipoOperacion>().ToTable("TiposOperaciones");
            modelBuilder.Entity<TipoOperacionMaterial>().ToTable("TiposOperacionesMateriales");

            modelBuilder.Entity<ImputacionOperario>(entity =>
            {
                entity.HasOne(i => i.Operacion)
                    .WithMany(o => o.Imputaciones)
                    .HasForeignKey(i => i.IdOperacion)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(i => i.Empleado)
                    .WithMany()
                    .HasForeignKey(i => i.IdEmpleado)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<MaquinaMaterial>()
                .HasKey(mm => new { mm.IdMaquina, mm.IdMaterial });

            modelBuilder.Entity<MaquinaMaterial>()
                .HasOne(mm => mm.Maquina)
                .WithMany(m => m.MaquinasMateriales)
                .HasForeignKey(mm => mm.IdMaquina)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MaquinaMaterial>()
                .HasOne(mm => mm.Material)
                .WithMany(mat => mat.MaquinasMateriales)
                .HasForeignKey(mm => mm.IdMaterial)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EmpleadoMaquina>()
                .HasKey(em => new { em.IdMaquina, em.IdEmpleado });

            modelBuilder.Entity<OperacionesOrden>()
                .HasOne(o => o.Seccion)
                .WithMany()
                .HasForeignKey(o => o.IdSeccion)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<OperacionesOrden>()
                .HasOne(o => o.Maquina)
                .WithMany()
                .HasForeignKey(o => o.IdMaquina)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<TipoOperacionMaterial>()
                .HasKey(tm => new { tm.IdTipoOperacion, tm.IdMaterial });

            modelBuilder.Entity<TipoOperacionMaterial>()
                .HasOne(tm => tm.TipoOperacion)
                .WithMany()
                .HasForeignKey(tm => tm.IdTipoOperacion)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TipoOperacionMaterial>()
                .HasOne(tm => tm.Material)
                .WithMany()
                .HasForeignKey(tm => tm.IdMaterial)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}