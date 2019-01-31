using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Library.Migrations
{
    public partial class createBorrowRequestedBook_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_BorrowBookRequested",
                table: "BorrowBookRequested");

            migrationBuilder.RenameTable(
                name: "BorrowBookRequested",
                newName: "BorrowRequestedBooks");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BorrowRequestedBooks",
                table: "BorrowRequestedBooks",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_BorrowRequestedBooks",
                table: "BorrowRequestedBooks");

            migrationBuilder.RenameTable(
                name: "BorrowRequestedBooks",
                newName: "BorrowBookRequested");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BorrowBookRequested",
                table: "BorrowBookRequested",
                column: "Id");
        }
    }
}
