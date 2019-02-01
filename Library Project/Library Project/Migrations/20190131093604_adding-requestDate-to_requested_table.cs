using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Library.Migrations
{
    public partial class addingrequestDateto_requested_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AnswerDate",
                table: "BorrowRequestedBooks",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequestDate",
                table: "BorrowRequestedBooks",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReturnDate",
                table: "BorrowRequestedBooks",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnswerDate",
                table: "BorrowRequestedBooks");

            migrationBuilder.DropColumn(
                name: "RequestDate",
                table: "BorrowRequestedBooks");

            migrationBuilder.DropColumn(
                name: "ReturnDate",
                table: "BorrowRequestedBooks");
        }
    }
}
