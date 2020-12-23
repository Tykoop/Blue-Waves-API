namespace Esentis.BlueWaves.Persistence.Abstractions
{
	using System;

	/// <summary>
	/// Exposes time stamping functionality.
	/// </summary>
	public interface ITimestamped
	{
		DateTimeOffset CreatedAt { get; set; }

		DateTimeOffset UpdatedAt { get; set; }
	}
}
