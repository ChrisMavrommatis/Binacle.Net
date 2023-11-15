using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace Binacle.Net.Api.Models
{
    public class PresetQueryRequest
    {
        public List<Box> Items { get; set; }

        public static PresetQueryRequest SampleRequest  = new PresetQueryRequest()
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

    public static class PresetQueryRequestSample
    {

    }
}
