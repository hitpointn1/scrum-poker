using Microsoft.EntityFrameworkCore.Migrations;

namespace PlanningPoker.Migrations
{
    public partial class hash_password : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Password",
                table: "PokerRooms",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "PokerRooms",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
