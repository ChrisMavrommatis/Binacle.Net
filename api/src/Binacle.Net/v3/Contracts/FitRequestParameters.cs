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

public interface IWithFittingParameters
{
	FitRequestParameters Parameters { get; set; }
}

[Description("Options that control how the fit check runs.")]
public class FitRequestParameters : 
	IWithAlgorithm,
	IOperationParameters,
	ILogParametersProvider
{
	[JsonConverter(typeof(JsonStringNullableEnumConverter))]
	[Description(SchemaDescriptions.Algorithm)]
	public required Algorithm? Algorithm { get; set; }

	public IReadOnlyList<string> ToLogParameters()
		=> [this.Operation.ToFastString(), this.Algorithm.ToFastString()];
	
	[JsonIgnore]
	public AlgorithmOperation Operation => AlgorithmOperation.Fitting;
}


internal class FitRequestParametersValidator : AbstractValidator<IWithFittingParameters>
{

	public FitRequestParametersValidator()
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
