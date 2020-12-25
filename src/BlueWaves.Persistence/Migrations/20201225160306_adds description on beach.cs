// < auto-generated />

using Microsoft.EntityFrameworkCore.Migrations;

namespace Esentis.BlueWaves.Persistence.Migrations
{
	public partial class addsdescriptiononbeach : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<string>(
				name: "Description",
				table: "Beaches",
				type: "text",
				nullable: false,
				defaultValue: "");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "Description",
				table: "Beaches");
		}
	}
}
