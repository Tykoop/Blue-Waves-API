// < auto-generated />

using Microsoft.EntityFrameworkCore.Migrations;

namespace Esentis.BlueWaves.Persistence.Migrations
{
	public partial class Adddeleteuserfunctionality : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<bool>(
				name: "IsDeleted",
				table: "Ratings",
				type: "boolean",
				nullable: false,
				defaultValue: false);

			migrationBuilder.AddColumn<bool>(
				name: "IsDeleted",
				table: "Favorites",
				type: "boolean",
				nullable: false,
				defaultValue: false);

			migrationBuilder.AddColumn<bool>(
				name: "IsDeleted",
				table: "AspNetUsers",
				type: "boolean",
				nullable: false,
				defaultValue: false);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "IsDeleted",
				table: "Ratings");

			migrationBuilder.DropColumn(
				name: "IsDeleted",
				table: "Favorites");

			migrationBuilder.DropColumn(
				name: "IsDeleted",
				table: "AspNetUsers");
		}
	}
}
