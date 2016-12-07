/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Piranha.EF.Migrations
{
    public partial class AddPageView : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Piranha_Posts_CategoryId",
                table: "Piranha_Posts");

            migrationBuilder.DropIndex(
                name: "IX_Piranha_PageFields_PageId",
                table: "Piranha_PageFields");

            migrationBuilder.DropIndex(
                name: "IX_Piranha_BlockFields_BlockId",
                table: "Piranha_BlockFields");

            migrationBuilder.AddColumn<string>(
                name: "View",
                table: "Piranha_Pages",
                maxLength: 255,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "View",
                table: "Piranha_Pages");

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_Posts_CategoryId",
                table: "Piranha_Posts",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_PageFields_PageId",
                table: "Piranha_PageFields",
                column: "PageId");

            migrationBuilder.CreateIndex(
                name: "IX_Piranha_BlockFields_BlockId",
                table: "Piranha_BlockFields",
                column: "BlockId");
        }
    }
}
