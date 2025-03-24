using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Qayimli.Repository.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReviewCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameEN = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NameAR = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ReviewOwnerEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PostDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReviewType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReviewCategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviews_ReviewCategories_ReviewCategoryId",
                        column: x => x.ReviewCategoryId,
                        principalTable: "ReviewCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReviewDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReviewContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReviewId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReviewDetails_Reviews_ReviewId",
                        column: x => x.ReviewId,
                        principalTable: "Reviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Votes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    User = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Rate = table.Column<double>(type: "float", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReviewDetailId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Votes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Votes_ReviewDetails_ReviewDetailId",
                        column: x => x.ReviewDetailId,
                        principalTable: "ReviewDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReviewDetails_ReviewId",
                table: "ReviewDetails",
                column: "ReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ReviewCategoryId",
                table: "Reviews",
                column: "ReviewCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Votes_ReviewDetailId",
                table: "Votes",
                column: "ReviewDetailId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Votes");

            migrationBuilder.DropTable(
                name: "ReviewDetails");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "ReviewCategories");
        }
    }
}
