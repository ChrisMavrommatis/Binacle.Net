using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.v4.Contracts.Pack;

#pragma warning disable CS1591
internal class PackCustomBinRequestExample : ISingleOpenApiExamplesProvider<PackCustomBinRequest>
{
    public IOpenApiExample<PackCustomBinRequest> GetExample()
    {
        return OpenApiExample.Create(
            "packcustombinrequest",
            "Pack Custom Bin Request",
            new PackCustomBinRequest()
            {
                Parameters = new OperationParameters()
                {
                    Algorithm = Algorithm.Best,
                    IncludeViPaqData = true,
                },
                Bin = new() { ID = "custom_bin", Length = 10, Width = 40, Height = 60 },
                Items = new List<Box>
                {
                    new() { ID = "box_1", Quantity = 2, Length = 2, Width = 5, Height = 10 },
                    new() { ID = "box_2", Quantity = 1, Length = 12, Width = 15, Height = 10 },
                    new() { ID = "box_3", Quantity = 1, Length = 12, Width = 10, Height = 15 },
                }
            });
    }
}