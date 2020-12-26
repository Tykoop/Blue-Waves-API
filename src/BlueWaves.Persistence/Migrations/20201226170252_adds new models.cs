// < auto-generated />

using System;

using Microsoft.EntityFrameworkCore.Migrations;

using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Esentis.BlueWaves.Persistence.Migrations
{
	public partial class addsnewmodels : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "Favorites",
				columns: table => new
				{
					Id = table.Column<long>(type: "bigint", nullable: false)
						.Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
					BeachId = table.Column<long>(type: "bigint", nullable: true),
					UserId = table.Column<Guid>(type: "uuid", nullable: true),
					CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
					UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Favorites", x => x.Id);
					table.ForeignKey(
						name: "FK_Favorites_AspNetUsers_UserId",
						column: x => x.UserId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Restrict);
					table.ForeignKey(
						name: "FK_Favorites_Beaches_BeachId",
						column: x => x.BeachId,
						principalTable: "Beaches",
						principalColumn: "Id",
						onDelete: ReferentialAction.Restrict);
				});

			migrationBuilder.CreateTable(
				name: "Ratings",
				columns: table => new
				{
					Id = table.Column<long>(type: "bigint", nullable: false)
						.Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
					BeachId = table.Column<long>(type: "bigint", nullable: true),
					UserId = table.Column<Guid>(type: "uuid", nullable: true),
					Rate = table.Column<int>(type: "integer", nullable: false),
					CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
					UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Ratings", x => x.Id);
					table.ForeignKey(
						name: "FK_Ratings_AspNetUsers_UserId",
						column: x => x.UserId,
						principalTable: "AspNetUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Restrict);
					table.ForeignKey(
						name: "FK_Ratings_Beaches_BeachId",
						column: x => x.BeachId,
						principalTable: "Beaches",
						principalColumn: "Id",
						onDelete: ReferentialAction.Restrict);
				});

			migrationBuilder.CreateIndex(
				name: "IX_Favorites_BeachId",
				table: "Favorites",
				column: "BeachId");

			migrationBuilder.CreateIndex(
				name: "IX_Favorites_UserId",
				table: "Favorites",
				column: "UserId");

			migrationBuilder.CreateIndex(
				name: "IX_Ratings_BeachId",
				table: "Ratings",
				column: "BeachId");

			migrationBuilder.CreateIndex(
				name: "IX_Ratings_UserId",
				table: "Ratings",
				column: "UserId");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "Favorites");

			migrationBuilder.DropTable(
				name: "Ratings");
		}
	}
}
