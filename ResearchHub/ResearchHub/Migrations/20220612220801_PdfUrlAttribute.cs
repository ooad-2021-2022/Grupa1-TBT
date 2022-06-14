using Microsoft.EntityFrameworkCore.Migrations;

namespace ResearchHub.Migrations
{
    public partial class PdfUrlAttribute : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "pdfFileUrl",
                table: "ResearchPaper",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "pdfFileUrl",
                table: "ResearchPaper");
        }
    }
}
