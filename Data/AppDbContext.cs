using Microsoft.EntityFrameworkCore;
using UniversityStudentManagement.Models;

namespace UniversityStudentManagement.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Group> Groups => Set<Group>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Student>(entity =>
        {
            entity.Property(student => student.Name).HasMaxLength(120).IsRequired();
            entity.Property(student => student.Email).HasMaxLength(254).IsRequired();
            entity.HasIndex(student => student.Email).IsUnique();
            entity.HasOne(student => student.Group)
                .WithMany(group => group.Students)
                .HasForeignKey(student => student.GroupId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.Property(group => group.Name).HasMaxLength(40).IsRequired();
            entity.HasIndex(group => group.Name).IsUnique();
            entity.HasData(
                new Group { Id = 1, Name = "Computer Science · CS-101" },
                new Group { Id = 2, Name = "Business · BUS-202" },
                new Group { Id = 3, Name = "Engineering · ENG-303" },
                new Group { Id = 4, Name = "Arts & Humanities · AH-110" });
        });
    }
}
