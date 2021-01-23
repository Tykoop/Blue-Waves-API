namespace Esentis.BlueWaves.Persistence.Model
{
	using System;

	using Esentis.BlueWaves.Persistence.Base;
	using Esentis.BlueWaves.Persistence.Helpers;
	using Esentis.BlueWaves.Persistence.Identity;

	using Kritikos.Configuration.Persistence.Abstractions;

	using NetTopologySuite.Geometries;

	public class Beach : Entity<long>, IAuditable<Guid>, IDeletable
	{
		public string Name { get; set; }

		public string Description { get; set; }

		public Point Coordinates { get; set; }

		public Country Country { get; set; }

		#region Implementation of IAuditable<Guid>
		public Guid CreatedBy { get; set; }

		public Guid UpdatedBy { get; set; }
		#endregion
		#region Implementation of IDeletable

		/// <inheritdoc />
		public bool IsDeleted { get; set; }
		#endregion
	}
}
