// < auto-generated />

using System;

using Microsoft.EntityFrameworkCore.Migrations;

namespace Esentis.BlueWaves.Persistence.Migrations
{
	public partial class addsrefreshtokenonuser : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<string>(
				name: "RefreshToken",
				table: "AspNetUsers",
				type: "text",
				nullable: false,
				defaultValue: "");

			migrationBuilder.AddColumn<DateTimeOffset>(
				name: "RefreshTokenExpiration",
				table: "AspNetUsers",
				type: "timestamp with time zone",
				nullable: false,
				defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "RefreshToken",
				table: "AspNetUsers");

			migrationBuilder.DropColumn(
				name: "RefreshTokenExpiration",
				table: "AspNetUsers");
		}
	}
}
