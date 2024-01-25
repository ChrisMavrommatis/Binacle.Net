using Binacle.Net.Api.Models;
using Binacle.Net.Api.Models.Requests;

namespace Binacle.Net.Api.Examples;

public class PresetQueryRequestExampleHolder : ISwaggerExampleHolder
{
    public static PresetQueryRequest Example =
        new PresetQueryRequest()
        {
            Items = new List<Box>()
            {
                new Box()
                {
                    ID = "box_1",
                    Quantity = 2,
                    Length = 2,
                    Width = 5,
                    Height = 10
                },
                new Box()
                {
                    ID = "box_2",
                    Quantity = 1,
                    Length = 12,
                    Width = 15,
                    Height = 10
                }
            }
        };
}
