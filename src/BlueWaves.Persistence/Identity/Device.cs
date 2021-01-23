namespace Esentis.BlueWaves.Persistence.Identity
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	using Esentis.BlueWaves.Persistence.Base;

	public class Device : Entity<long>
	{
		public string Name { get; set; }

		public BlueWavesUser User { get; set; }

		public Guid RefreshToken { get; set; }
	}
}
