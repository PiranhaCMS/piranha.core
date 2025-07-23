﻿using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Piranha.Data.EF.MySql.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class AddPostInformation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Excerpt",
                table: "Piranha_Posts",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PrimaryImageId",
                table: "Piranha_Posts",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Excerpt",
                table: "Piranha_Posts");

            migrationBuilder.DropColumn(
                name: "PrimaryImageId",
                table: "Piranha_Posts");
        }
    }
}
