namespace Esentis.BlueWaves.Persistence.Helpers
{
	using System;

	using Kritikos.Configuration.Persistence.Contracts.Behavioral;

	public class BlueWavesEntity<TKey> : BlueWavesJoinEntity, IEntity<TKey>
		where TKey : IComparable, IComparable<TKey>, IEquatable<TKey>
	{
		public TKey Id { get; set; }
	}
}
