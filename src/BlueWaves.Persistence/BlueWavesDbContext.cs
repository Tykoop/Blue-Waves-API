namespace Esentis.BlueWaves.Persistence
{
	using System;

	using Esentis.BlueWaves.Persistence.Helpers;
	using Esentis.BlueWaves.Persistence.Identity;
	using Esentis.BlueWaves.Persistence.Model;

	using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.Extensions.Logging;

	public class BlueWavesDbContext : IdentityDbContext<BlueWavesUser, BlueWavesRole, Guid>
	{
		private static readonly DateTimeOffset SeededAt = DateTime.Parse("23/01/2021");

		public BlueWavesDbContext(DbContextOptions<BlueWavesDbContext> options)
			: base(options)
		{
		}

		public DbSet<Beach> Beaches { get; init; }

		public DbSet<Favorite> Favorites { get; init; }

		public DbSet<Rating> Ratings { get; init; }

		public DbSet<Continent> Continents { get; init; }

		public DbSet<Country> Countries { get; init; }

		// Identity
		public DbSet<Device> Devices { get; init; }

		/// <inheritdoc />
		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			builder.Entity<BlueWavesRole>(entity =>
			{
				entity.HasData(new[]
				{
					new BlueWavesRole
					{
						CreatedAt = SeededAt,
						UpdatedAt = SeededAt,
						Id = Guid.Parse("bcb65d95-5cd1-4882-a1b5-f537cde80a22"),
						ConcurrencyStamp = "e683bff6-ff91-4c1e-af8b-203cdcf0ba3c",
						Name = RoleNames.Administrator,
						NormalizedName = RoleNames.Administrator,
					},
				});
			});

			builder.Entity<Beach>(entity => entity.HasQueryFilter(x => !x.IsDeleted));

			builder.Entity<Favorite>(entity =>
			{
				entity.HasOne(e => e.User).WithMany().HasForeignKey("UserId");
				entity.HasOne(e => e.Beach).WithMany().HasForeignKey("BeachId");
				entity.HasKey("UserId", "BeachId");
			});

			builder.Entity<Rating>(entity =>
			{
				entity.HasOne(e => e.User).WithMany().HasForeignKey("UserId");
				entity.HasOne(e => e.Beach).WithMany().HasForeignKey("BeachId");
				entity.HasKey("UserId", "BeachId");
			});
		}
	}
}
