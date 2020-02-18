using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Piranha.Data.EF.MySql.Migrations
{
    [NoCoverage]
    public partial class AddAliases : Migration
    {
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
                    SiteId = table.Column<Guid>(nullable: false),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piranha_Aliases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Piranha_Aliases_Piranha_Sites_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Piranha_Sites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_Aliases_SiteId_AliasUrl",
                table: "Piranha_Aliases",
                columns: new[] { "SiteId", "AliasUrl" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Piranha_Aliases");
        }
    }
}
