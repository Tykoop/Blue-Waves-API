namespace Esentis.BlueWaves.Web.Models
{
	using System.Threading.Tasks;

	using Esentis.BlueWaves.Web.Models.Dto;

	using Refit;

	public interface IBlueWavesApi
	{
		[Get("/beach/{id}")]
		Task<BeachDto> GetBeach(long id);
	}
}
