namespace Esentis.BlueWaves.Web.Models.Dto
{
	using System;

	public class FavoriteDto
	{
		public long Id { get; set; }

		public string BeachName { get; set; }

		public long BeachId { get; set; }

		public DateTimeOffset CreatedAt { get; set; }
	}
}
