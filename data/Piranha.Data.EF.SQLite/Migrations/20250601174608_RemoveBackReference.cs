using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Piranha.Data.EF.SQLite.Migrations
{
    /// <inheritdoc />
    public partial class RemoveBackReference : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Piranha_WorkflowSteps_Piranha_Workflows_WorkflowId",
                table: "Piranha_WorkflowSteps");

            migrationBuilder.AlterColumn<Guid>(
                name: "WorkflowId",
                table: "Piranha_WorkflowSteps",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AddForeignKey(
                name: "FK_Piranha_WorkflowSteps_Piranha_Workflows_WorkflowId",
                table: "Piranha_WorkflowSteps",
                column: "WorkflowId",
                principalTable: "Piranha_Workflows",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Piranha_WorkflowSteps_Piranha_Workflows_WorkflowId",
                table: "Piranha_WorkflowSteps");

            migrationBuilder.AlterColumn<Guid>(
                name: "WorkflowId",
                table: "Piranha_WorkflowSteps",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Piranha_WorkflowSteps_Piranha_Workflows_WorkflowId",
                table: "Piranha_WorkflowSteps",
                column: "WorkflowId",
                principalTable: "Piranha_Workflows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
