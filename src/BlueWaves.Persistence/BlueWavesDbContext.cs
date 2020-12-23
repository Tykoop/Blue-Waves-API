namespace Esentis.BlueWaves.Persistence
{
	using System;

	using Esentis.BlueWaves.Persistence.Identity;

	using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.Extensions.Logging;

	public class BlueWavesDbContext : IdentityDbContext<BlueWavesUser, BlueWavesRole, Guid>
	{
		private readonly ILoggerFactory loggerFactory;

		public BlueWavesDbContext(DbContextOptions<BlueWavesDbContext> options, ILoggerFactory? factory = null)
			: base(options)
		{
			loggerFactory = factory ?? Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory.Instance;
		}
	}
}
