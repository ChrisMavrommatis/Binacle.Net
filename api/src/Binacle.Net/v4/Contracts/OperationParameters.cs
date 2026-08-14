using System.Text.Json.Serialization;
using Binacle.Lib;
using Binacle.Lib.Abstractions.Algorithms;
using Binacle.Net.ExtensionMethods;
using Binacle.Net.Kernel.Logs.Models;
using Binacle.Net.Kernel.Serialization;
using FluentValidation;
using System.ComponentModel;

namespace Binacle.Net.v4.Contracts;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public interface IWithOperationParameters
{
    OperationParameters Parameters { get; set; }
}

[Description("Options that control how the operation runs.")]
public class OperationParameters :
    IWithAlgorithm,
    ILogParametersProvider,
    IOperationParameters
{
    [JsonConverter(typeof(JsonStringNullableEnumConverter))]
    [Description(SchemaDescriptions.Algorithm)]
    public required Algorithm? Algorithm { get; set; }

    [Description(SchemaDescriptions.IncludeViPaqData)]
    public bool IncludeViPaqData { get; set; } = false;

    public Binacle.Packing.Algorithm? GetAlgorithm()
    {
        if (!this.Algorithm.HasValue)
        {
            return null;
        }

        return this.Algorithm.Value switch
        {
            Contracts.Algorithm.FFD => Binacle.Packing.Algorithm.FFD,
            Contracts.Algorithm.WFD => Binacle.Packing.Algorithm.WFD,
            Contracts.Algorithm.BFD => Binacle.Packing.Algorithm.BFD,
            _ => null
        };
    }

    public IReadOnlyList<string> ToLogParameters()
        => [this.Operation.ToFastString(), this.AlgorithmFastString()];

    private string AlgorithmFastString()
    {
        if(!this.Algorithm.HasValue)
        {
            return "None";
        }
        return this.Algorithm.Value switch
        {
            Contracts.Algorithm.FFD => nameof(Contracts.Algorithm.FFD),
            Contracts.Algorithm.WFD => nameof(Contracts.Algorithm.WFD),
            Contracts.Algorithm.BFD => nameof(Contracts.Algorithm.BFD),
            Contracts.Algorithm.Best => nameof(Contracts.Algorithm.Best),
            _ => throw new NotSupportedException($"Algorithm {this.Algorithm.Value} is not supported.")
        };
    }

    [JsonIgnore]
    public AlgorithmOperation Operation { get; private set; }

    internal OperationParameters ForFittingOperation()
    {
        this.Operation = AlgorithmOperation.Fitting;
        return this;
    }

    internal OperationParameters ForPackingOperation()
    {
        this.Operation = AlgorithmOperation.Packing;
        return this;
    }
}

internal class OperationParametersValidator : AbstractValidator<IWithOperationParameters>
{
    public OperationParametersValidator()
    {
        RuleFor(x => x.Parameters)
            .NotNull();

        RuleFor(x => x.Parameters)
            .ChildRules(parametersValidator => { parametersValidator.Include(new AlgorithmValidator()); });
    }
}
