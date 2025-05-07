using System.ComponentModel.DataAnnotations;
using Binacle.Net.ServiceModule.v0.Resources;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;


namespace Binacle.Net.ServiceModule.v0.Contracts.Common;

internal class AccountId
{
	[FromRoute]
	public required string Id { get; set; } = null!;
	
	public Guid Value => Guid.Parse(Id);
}

internal class AccountIdValidator : AbstractValidator<AccountId>
{
	public AccountIdValidator()
	{
		RuleFor(x => x.Id)
			.NotNull()
			.NotEmpty()
			.Must(x => Guid.TryParse(x, out Guid id) && id != Guid.Empty)
			.WithMessage(ErrorMessage.IdMustBeGuid);
	}
}
