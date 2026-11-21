using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using UniversityStudentManagement.Data;

#nullable disable

namespace UniversityStudentManagement.Migrations;

[DbContext(typeof(AppDbContext))]
public partial class AppDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasAnnotation("ProductVersion", "9.0.10")
            .HasAnnotation("Relational:MaxIdentifierLength", 128);

        SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

        modelBuilder.Entity("UniversityStudentManagement.Models.Group", entity =>
        {
            entity.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
            SqlServerPropertyBuilderExtensions.UseIdentityColumn(entity.Property<int>("Id"));
            entity.Property<string>("Name").IsRequired().HasMaxLength(40).HasColumnType("nvarchar(40)");
            entity.HasKey("Id");
            entity.HasIndex("Name").IsUnique();
            entity.ToTable("Groups");
            entity.HasData(
                new { Id = 1, Name = "Computer Science · CS-101" },
                new { Id = 2, Name = "Business · BUS-202" },
                new { Id = 3, Name = "Engineering · ENG-303" },
                new { Id = 4, Name = "Arts & Humanities · AH-110" });
        });

        modelBuilder.Entity("UniversityStudentManagement.Models.Student", entity =>
        {
            entity.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
            SqlServerPropertyBuilderExtensions.UseIdentityColumn(entity.Property<int>("Id"));
            entity.Property<string>("Email").IsRequired().HasMaxLength(254).HasColumnType("nvarchar(254)");
            entity.Property<int>("GroupId").HasColumnType("int");
            entity.Property<string>("Name").IsRequired().HasMaxLength(120).HasColumnType("nvarchar(120)");
            entity.HasKey("Id");
            entity.HasIndex("Email").IsUnique();
            entity.HasIndex("GroupId");
            entity.ToTable("Students");
        });

        modelBuilder.Entity("UniversityStudentManagement.Models.Student", entity =>
        {
            entity.HasOne("UniversityStudentManagement.Models.Group", "Group")
                .WithMany("Students")
                .HasForeignKey("GroupId")
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
            entity.Navigation("Group");
        });

        modelBuilder.Entity("UniversityStudentManagement.Models.Group", entity => entity.Navigation("Students"));
    }
}
