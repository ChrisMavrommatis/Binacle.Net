using System.Text.Json.Serialization;
using Binacle.Lib;
using Binacle.Lib.Abstractions.Algorithms;
using Binacle.Net.ExtensionMethods;
using Binacle.Net.Kernel.Logs.Models;
using Binacle.Net.Kernel.Serialization;
using Binacle.Net.v3.ExtensionMethods;
using FluentValidation;
using System.ComponentModel;

namespace Binacle.Net.v3.Contracts;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public interface IWithPackingParameters
{
	PackRequestParameters Parameters { get; set; }
}

[Description("Options that control how the packing runs.")]
public class PackRequestParameters : 
	IWithAlgorithm,
	IOperationParameters,
	ILogParametersProvider
{
	[JsonConverter(typeof(JsonStringNullableEnumConverter))]
	[Description(SchemaDescriptions.Algorithm)]
	public required Algorithm? Algorithm { get; set; }

	[Description(SchemaDescriptions.IncludeViPaqData)]
	public bool IncludeViPaqData { get; set; } = false;

	public IReadOnlyList<string> ToLogParameters()
		=> [this.Operation.ToFastString(), this.Algorithm.ToFastString()];

	[JsonIgnore]
	public AlgorithmOperation Operation => AlgorithmOperation.Packing;
}

internal class PackRequestParametersValidator : AbstractValidator<IWithPackingParameters>
{

	public PackRequestParametersValidator()
	{
		RuleFor(x => x.Parameters)
			.NotNull();

		RuleFor(x => x.Parameters)
			.ChildRules(parametersValidator =>
			{
				parametersValidator.Include(new AlgorithmValidator());
			});
	}
}
