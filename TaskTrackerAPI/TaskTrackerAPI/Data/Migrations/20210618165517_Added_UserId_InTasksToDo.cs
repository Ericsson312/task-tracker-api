using Microsoft.EntityFrameworkCore.Migrations;

namespace TaskTrackerApi.Data.Migrations
{
    public partial class Added_UserId_InTasksToDo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "TasksToDo",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TasksToDo_UserId",
                table: "TasksToDo",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TasksToDo_AspNetUsers_UserId",
                table: "TasksToDo",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TasksToDo_AspNetUsers_UserId",
                table: "TasksToDo");

            migrationBuilder.DropIndex(
                name: "IX_TasksToDo_UserId",
                table: "TasksToDo");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "TasksToDo");
        }
    }
}
