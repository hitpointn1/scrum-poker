using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PlanningPoker.Migrations
{
    public partial class Nullables_and_IsOnline : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CloseDate",
                table: "PokerRooms",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AddColumn<bool>(
                name: "IsOnline",
                table: "Players",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsOnline",
                table: "Players");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CloseDate",
                table: "PokerRooms",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);
        }
    }
}
