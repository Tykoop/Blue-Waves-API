namespace Esentis.BlueWaves.Persistence.Helpers
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	public interface IDeletable
	{
		public bool IsDeleted { get; set; }
	}
}
