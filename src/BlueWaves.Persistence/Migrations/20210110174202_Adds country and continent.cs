// < auto-generated />

using System;

using Microsoft.EntityFrameworkCore.Migrations;

using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Esentis.BlueWaves.Persistence.Migrations
{
	public partial class Addscountryandcontinent : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<long>(
				name: "CountryId",
				table: "Beaches",
				type: "bigint",
				nullable: true);

			migrationBuilder.CreateTable(
				name: "Continents",
				columns: table => new
				{
					Id = table.Column<long>(type: "bigint", nullable: false)
						.Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
					Name = table.Column<string>(type: "text", nullable: false),
					Size = table.Column<long>(type: "bigint", nullable: false),
					CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
					UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Continents", x => x.Id);
				});

			migrationBuilder.CreateTable(
				name: "Countries",
				columns: table => new
				{
					Id = table.Column<long>(type: "bigint", nullable: false)
						.Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
					Name = table.Column<string>(type: "text", nullable: false),
					Code = table.Column<string>(type: "text", nullable: false),
					Currency = table.Column<string>(type: "text", nullable: false),
					Description = table.Column<string>(type: "text", nullable: false),
					ContinentId = table.Column<long>(type: "bigint", nullable: true),
					CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
					UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Countries", x => x.Id);
					table.ForeignKey(
						name: "FK_Countries_Continents_ContinentId",
						column: x => x.ContinentId,
						principalTable: "Continents",
						principalColumn: "Id",
						onDelete: ReferentialAction.Restrict);
				});

			migrationBuilder.CreateIndex(
				name: "IX_Beaches_CountryId",
				table: "Beaches",
				column: "CountryId");

			migrationBuilder.CreateIndex(
				name: "IX_Countries_ContinentId",
				table: "Countries",
				column: "ContinentId");

			migrationBuilder.AddForeignKey(
				name: "FK_Beaches_Countries_CountryId",
				table: "Beaches",
				column: "CountryId",
				principalTable: "Countries",
				principalColumn: "Id",
				onDelete: ReferentialAction.Restrict);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "FK_Beaches_Countries_CountryId",
				table: "Beaches");

			migrationBuilder.DropTable(
				name: "Countries");

			migrationBuilder.DropTable(
				name: "Continents");

			migrationBuilder.DropIndex(
				name: "IX_Beaches_CountryId",
				table: "Beaches");

			migrationBuilder.DropColumn(
				name: "CountryId",
				table: "Beaches");
		}
	}
}
