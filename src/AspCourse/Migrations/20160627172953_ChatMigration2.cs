using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AspCourse.Migrations
{
    public partial class ChatMigration2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Topics",
                nullable: true);

            migrationBuilder.RenameColumn(
                name: "IsCLosed",
                table: "Topics",
                newName: "IsClosed");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "Topics");

            migrationBuilder.RenameColumn(
                name: "IsClosed",
                table: "Topics",
                newName: "IsCLosed");
        }
    }
}
