using Microsoft.EntityFrameworkCore.Migrations;

namespace ResearchHub.Migrations
{
    public partial class NumOfRatings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "numberOfRatings",
                table: "ResearchPaper",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "numberOfRatings",
                table: "ResearchPaper");
        }
    }
}
