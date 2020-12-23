namespace Esentis.BlueWaves.Persistence.Base
{
	using System;

	using Esentis.BlueWaves.Persistence.Abstractions;

	public abstract class Entity<TKey> : IEntity<TKey>, ITimestamped
		where TKey : IComparable, IComparable<TKey>, IEquatable<TKey>
	{
		public TKey Id { get; set; }

		public DateTimeOffset CreatedAt { get; set; }

		public DateTimeOffset UpdatedAt { get; set; }
	}
}
