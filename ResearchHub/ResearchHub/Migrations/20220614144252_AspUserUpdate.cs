using Microsoft.EntityFrameworkCore.Migrations;

namespace ResearchHub.Migrations
{
    public partial class AspUserUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AspNetUserID",
                table: "User");

            migrationBuilder.AddColumn<string>(
                name: "aspNetID",
                table: "User",
                type: "nvarchar(max)",
                nullable: true);
        }
        
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "aspNetID",
                table: "User");

            migrationBuilder.AddColumn<int>(
                name: "AspNetUserID",
                table: "User",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
