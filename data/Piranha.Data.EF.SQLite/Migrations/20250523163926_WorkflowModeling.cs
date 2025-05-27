using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Piranha.Data.EF.SQLite.Migrations
{
    /// <inheritdoc />
    public partial class WorkflowModeling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "WorkflowId",
                table: "Piranha_Posts",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Piranha_Workflows",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CurrentStep = table.Column<int>(type: "INTEGER", nullable: false),
                    IsApproved = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piranha_Workflows", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Piranha_WorkflowSteps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    WorkflowId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Permission = table.Column<string>(type: "TEXT", nullable: true),
                    Reason = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piranha_WorkflowSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Piranha_WorkflowSteps_Piranha_Workflows_WorkflowId",
                        column: x => x.WorkflowId,
                        principalTable: "Piranha_Workflows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_Posts_WorkflowId",
                table: "Piranha_Posts",
                column: "WorkflowId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_WorkflowSteps_WorkflowId",
                table: "Piranha_WorkflowSteps",
                column: "WorkflowId");

            migrationBuilder.AddForeignKey(
                name: "FK_Piranha_Posts_Piranha_Workflows_WorkflowId",
                table: "Piranha_Posts",
                column: "WorkflowId",
                principalTable: "Piranha_Workflows",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Piranha_Posts_Piranha_Workflows_WorkflowId",
                table: "Piranha_Posts");

            migrationBuilder.DropTable(
                name: "Piranha_WorkflowSteps");

            migrationBuilder.DropTable(
                name: "Piranha_Workflows");

            migrationBuilder.DropIndex(
                name: "IX_Piranha_Posts_WorkflowId",
                table: "Piranha_Posts");

            migrationBuilder.DropColumn(
                name: "WorkflowId",
                table: "Piranha_Posts");
        }
    }
}
