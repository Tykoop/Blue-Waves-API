namespace Esentis.BlueWaves.Persistence.Helpers
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	using Esentis.BlueWaves.Persistence.Model;
	using Esentis.BlueWaves.Web.Models;

	public static class MappingExtensions
	{
		public static BeachDto toDto(this Beach beach) => new BeachDto(
				beach.Id,
				beach.Name,
				beach.Coordinates.X,
				beach.Coordinates.Y,
				beach.Description);
	}
}
