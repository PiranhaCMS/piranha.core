using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Piranha.Data.EF.SQLite.Migrations
{
    /// <inheritdoc />
    public partial class SwitchedToPages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Piranha_Posts_WorkflowId",
                table: "Piranha_Posts");

            migrationBuilder.AddColumn<Guid>(
                name: "WorkflowId",
                table: "Piranha_Pages",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_Posts_WorkflowId",
                table: "Piranha_Posts",
                column: "WorkflowId");

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_Pages_WorkflowId",
                table: "Piranha_Pages",
                column: "WorkflowId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Piranha_Pages_Piranha_Workflows_WorkflowId",
                table: "Piranha_Pages",
                column: "WorkflowId",
                principalTable: "Piranha_Workflows",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Piranha_Pages_Piranha_Workflows_WorkflowId",
                table: "Piranha_Pages");

            migrationBuilder.DropIndex(
                name: "IX_Piranha_Posts_WorkflowId",
                table: "Piranha_Posts");

            migrationBuilder.DropIndex(
                name: "IX_Piranha_Pages_WorkflowId",
                table: "Piranha_Pages");

            migrationBuilder.DropColumn(
                name: "WorkflowId",
                table: "Piranha_Pages");

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_Posts_WorkflowId",
                table: "Piranha_Posts",
                column: "WorkflowId",
                unique: true);
        }
    }
}
