﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Doctor_App.Infrastructure.Migrations
{
    public partial class ApprovalForDoctor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "Doctors",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "Doctors");
        }
    }
}
