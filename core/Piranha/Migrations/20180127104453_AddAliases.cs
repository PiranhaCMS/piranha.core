using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Piranha.Migrations
{
    public partial class AddAliases : Migration
    {
        [NoCoverage]
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Piranha_Aliases",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AliasUrl = table.Column<string>(maxLength: 256, nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    LastModified = table.Column<DateTime>(nullable: false),
                    RedirectUrl = table.Column<string>(maxLength: 256, nullable: false),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piranha_Aliases", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_Aliases_AliasUrl",
                table: "Piranha_Aliases",
                column: "AliasUrl",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Piranha_Aliases");
        }
    }
}
