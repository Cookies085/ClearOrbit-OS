using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ClearOrbit.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddServicesAndDivisions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BasePrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Unit = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DivisionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Services_Divisions_DivisionId",
                        column: x => x.DivisionId,
                        principalTable: "Divisions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Divisions",
                columns: new[] { "Id", "AccentColor", "CreatedAt", "IsActive", "Name", "OrganizationId", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000001"), "#1E90FF", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Academy", new Guid("00000000-0000-0000-0000-000000000001"), null },
                    { new Guid("10000000-0000-0000-0000-000000000002"), "#06B6D4", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Software", new Guid("00000000-0000-0000-0000-000000000001"), null },
                    { new Guid("10000000-0000-0000-0000-000000000003"), "#8B5CF6", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Games", new Guid("00000000-0000-0000-0000-000000000001"), null },
                    { new Guid("10000000-0000-0000-0000-000000000004"), "#F59E0B", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Designs", new Guid("00000000-0000-0000-0000-000000000001"), null },
                    { new Guid("10000000-0000-0000-0000-000000000005"), "#14B8A6", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Innovation Lab", new Guid("00000000-0000-0000-0000-000000000001"), null },
                    { new Guid("10000000-0000-0000-0000-000000000006"), "#3B82F6", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Digital Solutions", new Guid("00000000-0000-0000-0000-000000000001"), null },
                    { new Guid("10000000-0000-0000-0000-000000000007"), "#EC4899", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "MediaWorks", new Guid("00000000-0000-0000-0000-000000000001"), null },
                    { new Guid("10000000-0000-0000-0000-000000000008"), "#EAB308", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Training & Certification", new Guid("00000000-0000-0000-0000-000000000001"), null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Services_DivisionId_Name",
                table: "Services",
                columns: new[] { "DivisionId", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Services");

            migrationBuilder.DeleteData(
                table: "Divisions",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Divisions",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Divisions",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "Divisions",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "Divisions",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "Divisions",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000006"));

            migrationBuilder.DeleteData(
                table: "Divisions",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000007"));

            migrationBuilder.DeleteData(
                table: "Divisions",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000008"));
        }
    }
}
