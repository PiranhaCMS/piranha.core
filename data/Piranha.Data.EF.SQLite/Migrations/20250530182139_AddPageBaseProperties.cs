using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Piranha.Data.EF.SQLite.Migrations
{
    /// <inheritdoc />
    public partial class AddPageBaseProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Piranha_Posts_Piranha_Workflows_WorkflowId",
                table: "Piranha_Posts");

            migrationBuilder.DropIndex(
                name: "IX_Piranha_Posts_WorkflowId",
                table: "Piranha_Posts");

            migrationBuilder.DropColumn(
                name: "WorkflowId",
                table: "Piranha_Posts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "WorkflowId",
                table: "Piranha_Posts",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_Posts_WorkflowId",
                table: "Piranha_Posts",
                column: "WorkflowId");

            migrationBuilder.AddForeignKey(
                name: "FK_Piranha_Posts_Piranha_Workflows_WorkflowId",
                table: "Piranha_Posts",
                column: "WorkflowId",
                principalTable: "Piranha_Workflows",
                principalColumn: "Id");
        }
    }
}
