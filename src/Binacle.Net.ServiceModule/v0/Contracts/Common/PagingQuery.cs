using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Binacle.Net.ServiceModule.v0.Contracts.Common;


internal class PagingQuery
{
	[FromQuery(Name = "pg")]
	public int? Pg { get; set; }
	[FromQuery(Name = "pz")]
	public int? Pz { get; set; }

	public int PageNumber => this.Pg ?? 1;
	public int PageSize => this.Pz ?? 10;
}

internal class PagingQueryValidator : AbstractValidator<PagingQuery>
{
	public PagingQueryValidator()
	{
		When(x => x.Pg.HasValue, () =>
		{
			RuleFor(x => x.Pg)
				.NotNull()
				.GreaterThanOrEqualTo(1);
		});
		
		When(x => x.Pz.HasValue, () =>
		{
			RuleFor(x => x.Pz)
				.NotNull()
				.GreaterThanOrEqualTo(1);
		});
	}
}

