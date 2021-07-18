using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TaskTrackerApi.Data.Migrations
{
    public partial class Added_BoardIt_ToCard : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cards_Boards_BoardId",
                table: "Cards");

            migrationBuilder.DropIndex(
                name: "IX_Cards_BoardId",
                table: "Cards");

            migrationBuilder.AlterColumn<string>(
                name: "BoardId",
                table: "Cards",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BoardId1",
                table: "Cards",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cards_BoardId1",
                table: "Cards",
                column: "BoardId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Cards_Boards_BoardId1",
                table: "Cards",
                column: "BoardId1",
                principalTable: "Boards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cards_Boards_BoardId1",
                table: "Cards");

            migrationBuilder.DropIndex(
                name: "IX_Cards_BoardId1",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "BoardId1",
                table: "Cards");

            migrationBuilder.AlterColumn<Guid>(
                name: "BoardId",
                table: "Cards",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cards_BoardId",
                table: "Cards",
                column: "BoardId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cards_Boards_BoardId",
                table: "Cards",
                column: "BoardId",
                principalTable: "Boards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
