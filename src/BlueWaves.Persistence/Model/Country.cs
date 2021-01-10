namespace Esentis.BlueWaves.Persistence.Model
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	using Esentis.BlueWaves.Persistence.Base;

	public class Country : Entity<long>
	{
		public string Name { get; set; }

		public string Code { get; set; }

		public string Currency { get; set; }

		public string Description { get; set; }

		public Continent Continent { get; set; }
	}
}
