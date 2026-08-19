using Binacle.Net.ServiceModule.v0.Resources;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Binacle.Net.ServiceModule.v0.Contracts.Common;

internal class PageQuery
{
	internal const int DefaultPageSize = 50;

	// The cap is the only thing between one admin URL and a full table read on every backend.
	internal const int MaxPageSize = 200;

	[FromQuery]
	public int? Page { get; set; }

	[FromQuery]
	public int? PageSize { get; set; }

	[FromQuery]
	public bool? AllowDeleted { get; set; }

	public int PageOrDefault => this.Page ?? 1;
	public int PageSizeOrDefault => this.PageSize ?? DefaultPageSize;
	public int Skip => (this.PageOrDefault - 1) * this.PageSizeOrDefault;
}

internal class PageQueryValidator : AbstractValidator<PageQuery>
{
	public PageQueryValidator()
	{
		RuleFor(x => x.Page)
			.GreaterThanOrEqualTo(1)
			.When(x => x.Page.HasValue)
			.WithMessage(ErrorMessage.PageMustBePositive);

		RuleFor(x => x.PageSize)
			.InclusiveBetween(1, PageQuery.MaxPageSize)
			.When(x => x.PageSize.HasValue)
			.WithMessage(ErrorMessage.PageSizeOutOfRange(PageQuery.MaxPageSize));
	}
}

internal class PageQueryValidationProblemExample : ValidationProblemResponseExample
{
	public override Dictionary<string, string[]> GetErrors()
	{
		return new Dictionary<string, string[]>()
		{
			{ "PageSize", [ErrorMessage.PageSizeOutOfRange(PageQuery.MaxPageSize)] }
		};
	}
}
