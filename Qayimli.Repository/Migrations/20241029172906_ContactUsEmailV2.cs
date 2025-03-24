using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Qayimli.Repository.Migrations
{
    /// <inheritdoc />
    public partial class ContactUsEmailV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "ContactUsEmails",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContactUsEmails",
                table: "ContactUsEmails",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ContactUsEmails",
                table: "ContactUsEmails");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ContactUsEmails");
        }
    }
}
