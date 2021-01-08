// < auto-generated />
using System;

using Microsoft.EntityFrameworkCore.Migrations;

namespace Esentis.BlueWaves.Persistence.Migrations
{
	public partial class AddsaddedByonbeach : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<Guid>(
				name: "AddedById",
				table: "Beaches",
				type: "uuid",
				nullable: true);

			migrationBuilder.CreateIndex(
				name: "IX_Beaches_AddedById",
				table: "Beaches",
				column: "AddedById");

			migrationBuilder.AddForeignKey(
				name: "FK_Beaches_AspNetUsers_AddedById",
				table: "Beaches",
				column: "AddedById",
				principalTable: "AspNetUsers",
				principalColumn: "Id",
				onDelete: ReferentialAction.Restrict);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "FK_Beaches_AspNetUsers_AddedById",
				table: "Beaches");

			migrationBuilder.DropIndex(
				name: "IX_Beaches_AddedById",
				table: "Beaches");

			migrationBuilder.DropColumn(
				name: "AddedById",
				table: "Beaches");
		}
	}
}
