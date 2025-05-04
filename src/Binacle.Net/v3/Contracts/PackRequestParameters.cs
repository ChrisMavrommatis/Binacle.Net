using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Binacle.Net.Kernel.Serialization;
using Binacle.Net.Models;
using FluentValidation;

namespace Binacle.Net.v3.Contracts;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public interface IWithPackingParameters
{
	PackRequestParameters Parameters { get; set; }
}

public class PackRequestParameters : IWithAlgorithm
{
	// TODO: Investigate this as with this it fails
	[Required]
	[JsonConverter(typeof(JsonStringNullableEnumConverter<Nullable<Algorithm>>))]
	public Algorithm? Algorithm { get; set; }
}

internal class PackRequestParametersValidator : AbstractValidator<IWithPackingParameters>
{

	public PackRequestParametersValidator()
	{
		RuleFor(x => x.Parameters)
			.NotNull();

		RuleFor(x => x.Parameters!)
			.ChildRules(parametersValidator =>
			{
				parametersValidator.Include(new AlgorithmValidator());
			});
	}
}
