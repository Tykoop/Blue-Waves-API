namespace Esentis.BlueWaves.Web.Models.Dto
{
	using System;

	public class RatingDto
	{
		public double Rate { get; set; }

		public DateTimeOffset CreatedAt { get; set; }

		public string BeachName { get; set; }

		public long BeachId { get; set; }

		public string Review { get; set; }
	}
}
