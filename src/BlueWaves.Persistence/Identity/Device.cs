namespace Esentis.BlueWaves.Persistence.Identity
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	using Esentis.BlueWaves.Persistence.Base;
	using Esentis.BlueWaves.Persistence.Helpers;

	public class Device : Entity<long>, IDeletable
	{
		public string Name { get; set; }

		public BlueWavesUser User { get; set; }

		public Guid RefreshToken { get; set; }

		#region Implementation of IDeletable

		/// <inheritdoc />
		public bool IsDeleted { get; set; }
		#endregion
	}
}
