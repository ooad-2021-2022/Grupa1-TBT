using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ResearchHub.Migrations
{
    public partial class RequestsAdder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Requests",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    requesterID = table.Column<int>(type: "int", nullable: false),
                    requesteeID = table.Column<int>(type: "int", nullable: false),
                    timeRequestMade = table.Column<DateTime>(type: "datetime2", nullable: false),
                    requestBody = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Requests", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Requests");
        }
    }
}
