// < auto-generated />

using Microsoft.EntityFrameworkCore.Migrations;

namespace Esentis.BlueWaves.Persistence.Migrations
{
	public partial class AdddeleteDevice : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<bool>(
				name: "IsDeleted",
				table: "Devices",
				type: "boolean",
				nullable: false,
				defaultValue: false);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "IsDeleted",
				table: "Devices");
		}
	}
}
