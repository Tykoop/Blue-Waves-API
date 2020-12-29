namespace Esentis.BlueWaves.Web.Api.Helpers
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using System.Linq;
	using System.Threading.Tasks;

	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter | AttributeTargets.Field)]
	public class PositiveNumberValidator : ValidationAttribute
	{
		public override bool IsValid(object value) =>
			value is int input && input > 0;
	}
}
