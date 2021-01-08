namespace Esentis.BlueWaves.Persistence.Helpers
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading;
	using System.Threading.Tasks;

	using Kritikos.Configuration.Persistence.Abstractions;

	using Microsoft.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore.Diagnostics;

	public class TimeStampSaveChangesInterceptor : SaveChangesInterceptor
	{
		public override InterceptionResult<int> SavingChanges(DbContextEventData eventData,
			InterceptionResult<int> result)
		{
			var entries = eventData.Context.ChangeTracker.Entries<ITimestamped>();
			var now = DateTimeOffset.Now;

			foreach (var entry in entries.Where(x => x.State == EntityState.Added))
			{
				entry.Entity.CreatedAt = now;
				entry.Entity.UpdatedAt = now;
			}

			foreach (var entry in entries.Where(x => x.State == EntityState.Modified))
			{
				entry.Entity.UpdatedAt = now;
			}

			return base.SavingChanges(eventData, result);
		}

		public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
			InterceptionResult<int> result,
			CancellationToken cancellationToken = new CancellationToken())
		{
			var entries = eventData.Context.ChangeTracker.Entries<ITimestamped>();
			var now = DateTimeOffset.Now;

			foreach (var entry in entries.Where(x => x.State == EntityState.Added))
			{
				entry.Entity.CreatedAt = now;
				entry.Entity.UpdatedAt = now;
			}

			foreach (var entry in entries.Where(x => x.State == EntityState.Modified))
			{
				entry.Entity.UpdatedAt = now;
			}

			return base.SavingChangesAsync(eventData, result, cancellationToken);
		}
	}
}
