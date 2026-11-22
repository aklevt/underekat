using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UnderEkat.Api.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "collections",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    owner_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(55)", maxLength: 55, nullable: false),
                    is_public = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_collections", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "places",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    author_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(65)", maxLength: 65, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    address = table.Column<string>(type: "text", nullable: false),
                    latitude = table.Column<double>(type: "double precision", nullable: false),
                    longitude = table.Column<double>(type: "double precision", nullable: false),
                    access = table.Column<int>(type: "integer", nullable: false),
                    guard = table.Column<int>(type: "integer", nullable: false),
                    tags = table.Column<string[]>(type: "text[]", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_places", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "collection_places",
                columns: table => new
                {
                    collection_id = table.Column<Guid>(type: "uuid", nullable: false),
                    place_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_collection_places", x => new { x.collection_id, x.place_id });
                    table.ForeignKey(
                        name: "FK_collection_places_collections_collection_id",
                        column: x => x.collection_id,
                        principalTable: "collections",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_collection_places_places_place_id",
                        column: x => x.place_id,
                        principalTable: "places",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_collection_places_place_id",
                table: "collection_places",
                column: "place_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "collection_places");

            migrationBuilder.DropTable(
                name: "collections");

            migrationBuilder.DropTable(
                name: "places");
        }
    }
}
