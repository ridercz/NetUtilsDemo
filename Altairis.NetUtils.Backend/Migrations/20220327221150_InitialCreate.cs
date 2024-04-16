using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Altairis.NetUtils.Backend.Migrations; 
public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "TraceJobs",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Status = table.Column<int>(type: "int", nullable: false),
                DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                DateStarted = table.Column<DateTime>(type: "datetime2", nullable: true),
                DateCompleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                Host = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Result = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_TraceJobs", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "TraceJobs");
    }
}
