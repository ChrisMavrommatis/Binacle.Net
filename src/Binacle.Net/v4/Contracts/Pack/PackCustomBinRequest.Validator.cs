using FluentValidation;

namespace Binacle.Net.v4.Contracts.Pack;

#pragma warning disable CS1591
internal class PackCustomBinRequestValidator : AbstractValidator<PackCustomBinRequest>
{
    public PackCustomBinRequestValidator()
    {
        Include(new OperationParametersValidator());
        Include(new BinValidator());
        Include(new ItemsValidator());
    }
}