using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Qayimli.Repository.Migrations
{
    /// <inheritdoc />
    public partial class ChangeVoteUserToVoteOwnerEmailAndAddVoteDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "User",
                table: "Votes",
                newName: "VoteOwnerEmail");

            migrationBuilder.AddColumn<DateTime>(
                name: "VoteDate",
                table: "Votes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2024, 10, 16, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VoteDate",
                table: "Votes");

            migrationBuilder.RenameColumn(
                name: "VoteOwnerEmail",
                table: "Votes",
                newName: "User");
        }
    }
}
