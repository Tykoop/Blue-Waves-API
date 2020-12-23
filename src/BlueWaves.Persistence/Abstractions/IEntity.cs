namespace Esentis.BlueWaves.Persistence.Abstractions
{
	using System;

	public interface IEntity<TKey>
	where TKey : IComparable, IComparable<TKey>, IEquatable<TKey>
	{
		TKey Id { get; set; }
	}
}
