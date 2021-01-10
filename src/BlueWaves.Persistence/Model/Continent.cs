namespace Esentis.BlueWaves.Persistence.Model
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	using Esentis.BlueWaves.Persistence.Base;

	public class Continent : Entity<long>
	{
		public string Name { get; set; }

		public long Size { get; set; }
	}
}
