namespace Esentis.BlueWaves.Persistence
{
	using System;

	using Esentis.BlueWaves.Persistence.Helpers;
	using Esentis.BlueWaves.Persistence.Identity;
	using Esentis.BlueWaves.Persistence.Model;

	using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore;

	public class BlueWavesDbContext : IdentityDbContext<BlueWavesUser, BlueWavesRole, Guid>
	{
		private static readonly DateTime SeededAt = DateTime.Parse("23/01/2021");

		public BlueWavesDbContext(DbContextOptions<BlueWavesDbContext> options)
			: base(options)
		{
		}

		public DbSet<Beach> Beaches { get; init; }

		public DbSet<Favorite> Favorites { get; init; }

		public DbSet<Rating> Ratings { get; init; }

		public DbSet<Continent> Continents { get; init; }

		public DbSet<Country> Countries { get; init; }

		public DbSet<Device> Devices { get; init; }

		public DbSet<Image> Images { get; set; }

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
					new BlueWavesRole
					{
						CreatedAt = SeededAt,
						UpdatedAt = SeededAt,
						Id = Guid.Parse("0e19eb6e-2a11-4fa3-9a49-890dfa4a1a85"),
						ConcurrencyStamp = "55f3c264-256f-4b1a-bec4-a411dbab3e64",
						Name = RoleNames.Member,
						NormalizedName = RoleNames.Member,
					},
				});
			});

			builder.Entity<Beach>(entity => entity.HasQueryFilter(x => !x.IsDeleted));

			builder.Entity<Rating>(entity =>
			{
				entity.HasQueryFilter(x => !x.IsDeleted);
				entity.HasOne(e => e.Beach)
					.WithMany(e => e.Ratings)
					.OnDelete(DeleteBehavior.Restrict);
			});

			builder.Entity<Image>(entity =>
			{
				entity.HasQueryFilter(x => !x.IsDeleted);
				entity.HasOne(e => e.Beach)
					.WithMany(e => e.Images)
					.OnDelete(DeleteBehavior.Restrict);
			});
		}
	}
}
