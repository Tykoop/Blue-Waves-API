namespace Esentis.BlueWaves.Persistence.Abstractions
{
	using System;

	public interface IAuditable<T>
	where T : IComparable, IComparable<T>, IEquatable<T>
	{
		T CreatedBy { get; set; }

		T UpdatedBy { get; set; }
	}
}
