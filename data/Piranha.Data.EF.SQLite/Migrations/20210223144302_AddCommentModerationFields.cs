using Microsoft.EntityFrameworkCore.Migrations;

namespace Piranha.Data.EF.SQLite.Migrations
{
    public partial class AddCommentModerationFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.AddColumn<int>(
				name: "Status",
				table: "Piranha_PostComments",
				nullable: false,
                defaultValue: 1);
				
			migrationBuilder.AddColumn<int>(
				name: "Status",
				table: "Piranha_PageComments",
				nullable: false,
				defaultValue: 1);
				
			migrationBuilder.Sql(@"
				UPDATE Piranha_PostComments
				SET Status = IsApproved");
			
			migrationBuilder.Sql(@"
				UPDATE Piranha_PageComments
				SET Status = IsApproved");
				
			migrationBuilder.DropColumn(
				name: "IsApproved",
				table: "Piranha_PostComments");
				
			migrationBuilder.DropColumn(
				name: "IsApproved",
				table: "Piranha_PageComments");
				
            migrationBuilder.AddColumn<string>(
                name: "StatusReason",
                table: "Piranha_PostComments",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StatusReason",
                table: "Piranha_PageComments",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StatusReason",
                table: "Piranha_PostComments");

            migrationBuilder.DropColumn(
                name: "StatusReason",
                table: "Piranha_PageComments");

			migrationBuilder.AddColumn<bool>(
				name: "IsApproved",
				table: "Piranha_PostComments",
				nullable: false,
				defaultValue: true);
				
			migrationBuilder.AddColumn<bool>(
				name: "IsApproved",
				table: "Piranha_PageComments",
				nullable: false,
				defaultValue: true);
				
			migrationBuilder.Sql(@"
				UPDATE Piranha_PostComments
				SET IsApproved = Status");
			
			migrationBuilder.Sql(@"
				UPDATE Piranha_PageComments
				SET IsApproved = Status");
				
			migrationBuilder.DropColumn(
				name: "Status",
				table: "Piranha_PostComments");
				
			migrationBuilder.DropColumn(
				name: "Status",
				table: "Piranha_PageComments");
				
				
        }
    }
}
