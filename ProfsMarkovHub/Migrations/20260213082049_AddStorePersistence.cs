using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProfsMarkovHub.Migrations
{
    /// <inheritdoc />
    public partial class AddStorePersistence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StoreItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ExternalId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 160, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ImageUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Cost = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    LastSyncedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StoreRedemptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ExternalId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    StoreItemExternalId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    StoreItemName = table.Column<string>(type: "TEXT", maxLength: 160, nullable: false),
                    ViewerName = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    Cost = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    RedeemedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreRedemptions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Articles_Slug",
                table: "Articles",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoreItems_ExternalId",
                table: "StoreItems",
                column: "ExternalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoreRedemptions_ExternalId",
                table: "StoreRedemptions",
                column: "ExternalId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StoreItems");

            migrationBuilder.DropTable(
                name: "StoreRedemptions");

            migrationBuilder.DropIndex(
                name: "IX_Articles_Slug",
                table: "Articles");
        }
    }
}
