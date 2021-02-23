using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Octoller.BotBox.Web.Migrations
{
    public partial class ModifiedModelsAndContextAndStore : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdateAt",
                table: "Communities");

            migrationBuilder.DropColumn(
                name: "UpdateAt",
                table: "Accounts");

            migrationBuilder.RenameColumn(
                name: "UpdateBy",
                table: "Communities",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "CreateBy",
                table: "Communities",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "UpdateBy",
                table: "Accounts",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "CreateBy",
                table: "Accounts",
                newName: "CreatedBy");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Communities",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Accounts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Communities");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Accounts");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "Communities",
                newName: "UpdateBy");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "Communities",
                newName: "CreateBy");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "Accounts",
                newName: "UpdateBy");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "Accounts",
                newName: "CreateBy");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateAt",
                table: "Communities",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateAt",
                table: "Accounts",
                type: "datetime2",
                nullable: true);
        }
    }
}
