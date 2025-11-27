using Microsoft.EntityFrameworkCore;
using SmartCrm.Domain.Entities;
using SmartCrm.Domain.Entities.Attendances;
using SmartCrm.Domain.Entities.Groups;
using SmartCrm.Domain.Entities.Payments;
using SmartCrm.Domain.Entities.Students;
using SmartCrm.Domain.Entities.Teachers;
using SmartCrm.Domain.Entities.Users;
using SmartCrm.Domain.Enums;

namespace SmartCrm.Data.DbContexts
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Attendance> Attendances { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var superAdminId = new Guid("a1b2c3d4-e5f6-7788-9900-aabbccddeeff");

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = superAdminId,
                    Username = "Tommy", 
                    PasswordHash = "cd59680a1bb16445f3cbcec2ef65dcb53cd6f18832a5087f4b13dcda9d8fa409", // string tayyar
                    Role = Role.SuperAdministrator,
                    IsActive = true,
                    CreatedDate = new DateTime(2024, 05, 20, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedDate = new DateTime(2024, 05, 20, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<Teacher>()
                .HasIndex(t => t.PhoneNumber)
                .IsUnique();

            modelBuilder.Entity<Student>()
                .HasIndex(s => s.PhoneNumber)
                .IsUnique();

            modelBuilder.Entity<Group>()
                .HasOne(g => g.Teacher)
                .WithMany(t => t.Groups)
                .HasForeignKey(g => g.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Student>()
                .HasOne(s => s.Group)
                .WithMany(g => g.Students)
                .HasForeignKey(s => s.GroupId)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Student)
                .WithMany(s => s.Payments)
                .HasForeignKey(p => p.StudentId)
                .OnDelete(DeleteBehavior.Cascade); 
        }

        public override int SaveChanges()
        {
            AddTimestamps();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            AddTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void AddTimestamps()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is BaseEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                var now = DateTime.UtcNow;
                var entity = (BaseEntity)entityEntry.Entity;

                entity.UpdatedDate = now;

                if (entityEntry.State == EntityState.Added)
                {
                    entity.CreatedDate = now;
                }
            }
        }
    }
}