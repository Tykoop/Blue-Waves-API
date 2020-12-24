namespace Esentis.BlueWaves.Persistence
{
	using System;

	using Esentis.BlueWaves.Persistence.Identity;
	using Esentis.BlueWaves.Persistence.Model;

	using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.Extensions.Logging;

	public class BlueWavesDbContext : IdentityDbContext<BlueWavesUser, BlueWavesRole, Guid>
	{
		public BlueWavesDbContext(DbContextOptions<BlueWavesDbContext> options)
			: base(options)
		{
		}

		public DbSet<Beach> Beaches { get; init; }

		/// <inheritdoc />
		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			//builder.Entity<Beach>(x =>
			//{
			//	x.Property(y => y.Coordinates).HasColumnType("geometry (point)");
			//});
		}
	}
}
