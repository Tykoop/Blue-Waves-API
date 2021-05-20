namespace Esentis.BlueWaves.Web.Models.Dto
{
	using System.Collections.Generic;

	public class AddImageDto
	{
		public List<string> Images { get; set; }

		public long BeachId { get; set; }
	}
}
