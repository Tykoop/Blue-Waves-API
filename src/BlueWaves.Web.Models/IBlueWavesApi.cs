namespace Esentis.BlueWaves.Web.Models
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	using Refit;

	public interface IBlueWavesApi
	{
		[Get("/beach/{id}")]
		Task<BeachDto> GetBeach(long id);
	}
}
