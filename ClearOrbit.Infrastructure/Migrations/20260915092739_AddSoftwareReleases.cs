using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClearOrbit.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSoftwareReleases : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ReleaseId",
                table: "Features",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Releases",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Version = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReleaseNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DivisionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PlannedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReleasedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReleaseManagerUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Releases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Releases_Divisions_DivisionId",
                        column: x => x.DivisionId,
                        principalTable: "Divisions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Releases_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Releases_Users_ReleaseManagerUserId",
                        column: x => x.ReleaseManagerUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Features_ReleaseId",
                table: "Features",
                column: "ReleaseId");

            migrationBuilder.CreateIndex(
                name: "IX_Releases_Code",
                table: "Releases",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Releases_DivisionId",
                table: "Releases",
                column: "DivisionId");

            migrationBuilder.CreateIndex(
                name: "IX_Releases_ProjectId_Version",
                table: "Releases",
                columns: new[] { "ProjectId", "Version" },
                unique: true,
                filter: "[ProjectId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Releases_ReleaseManagerUserId",
                table: "Releases",
                column: "ReleaseManagerUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Features_Releases_ReleaseId",
                table: "Features",
                column: "ReleaseId",
                principalTable: "Releases",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Features_Releases_ReleaseId",
                table: "Features");

            migrationBuilder.DropTable(
                name: "Releases");

            migrationBuilder.DropIndex(
                name: "IX_Features_ReleaseId",
                table: "Features");

            migrationBuilder.DropColumn(
                name: "ReleaseId",
                table: "Features");
        }
    }
}
