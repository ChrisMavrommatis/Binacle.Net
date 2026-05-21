using System.Text.Json.Serialization;
using Binacle.Lib;
using Binacle.Lib.Abstractions.Algorithms;
using Binacle.Net.ExtensionMethods;
using Binacle.Net.Kernel.Logs.Models;
using Binacle.Net.Kernel.Serialization;
using Binacle.Net.v3.ExtensionMethods;
using FluentValidation;

namespace Binacle.Net.v3.Contracts;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public interface IWithPackingParameters
{
	PackRequestParameters Parameters { get; set; }
}

public class PackRequestParameters : 
	IWithAlgorithm, 
	IOperationParameters, 
	ILogConvertible
{
	[JsonConverter(typeof(JsonStringNullableEnumConverter))]
	public required Algorithm? Algorithm { get; set; }

	public bool IncludeViPaqData { get; set; } = false;
	
	public object ConvertToLogObject()
	{
		// Algorithm Is Always added
		var paramsCount = 2;

		var parameters = new string[paramsCount];

		parameters[--paramsCount] = this.Algorithm.ToFastString();
		parameters[--paramsCount] = this.Operation.ToFastString();
		
		return parameters;
	}

	[JsonIgnore]
	public AlgorithmOperation Operation => AlgorithmOperation.Packing;
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
