namespace Esentis.BlueWaves.Web.Api.Helpers
{
	using System;
	using System.ComponentModel.DataAnnotations;

	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter | AttributeTargets.Field)]
	public class ItemPerPageValidator : ValidationAttribute
	{
		public override bool IsValid(object value) =>
			value is int input && input >= 10;
	}
}
