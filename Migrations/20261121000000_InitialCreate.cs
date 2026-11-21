using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using UniversityStudentManagement.Data;

#nullable disable

namespace UniversityStudentManagement.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20261121000000_InitialCreate")]
public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Groups",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_Groups", x => x.Id));

        migrationBuilder.CreateTable(
            name: "Students",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                Email = table.Column<string>(type: "nvarchar(254)", maxLength: 254, nullable: false),
                GroupId = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Students", x => x.Id);
                table.ForeignKey("FK_Students_Groups_GroupId", x => x.GroupId, "Groups", "Id", onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.Sql("""
            SET IDENTITY_INSERT [Groups] ON;
            INSERT INTO [Groups] ([Id], [Name]) VALUES
                (1, N'Computer Science · CS-101'),
                (2, N'Business · BUS-202'),
                (3, N'Engineering · ENG-303'),
                (4, N'Arts & Humanities · AH-110');
            SET IDENTITY_INSERT [Groups] OFF;
            """);

        migrationBuilder.CreateIndex(name: "IX_Groups_Name", table: "Groups", column: "Name", unique: true);
        migrationBuilder.CreateIndex(name: "IX_Students_Email", table: "Students", column: "Email", unique: true);
        migrationBuilder.CreateIndex(name: "IX_Students_GroupId", table: "Students", column: "GroupId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "Students");
        migrationBuilder.DropTable(name: "Groups");
    }
}
