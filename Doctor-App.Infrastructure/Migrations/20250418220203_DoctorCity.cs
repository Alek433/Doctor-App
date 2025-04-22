using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Doctor_App.Infrastructure.Migrations
{
    public partial class DoctorCity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "Doctors");
        }
    }
}
