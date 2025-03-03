using Materials.Models;
using Microsoft.EntityFrameworkCore;

namespace Materials.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Material> Materials { get; set; }
        public DbSet<Equipment> Equipments { get; set; }
        public DbSet<MaterialRecord> MaterialRecords { get; set; }
        public DbSet<MaterialLog> MaterialLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MaterialRecord>()
                .HasOne(mr => mr.Material)
                .WithMany(m => m.MaterialRecords)
                .HasForeignKey(mr => mr.MaterialId);

            modelBuilder.Entity<MaterialRecord>()
                .HasOne(mr => mr.Equipment)
                .WithMany(e => e.MaterialRecords)
                .HasForeignKey(mr => mr.EquipmentId);

            modelBuilder.Entity<MaterialRecord>()
                .HasOne(mr => mr.User)
                .WithMany(u => u.MaterialRecords)
                .HasForeignKey(mr => mr.UserId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
