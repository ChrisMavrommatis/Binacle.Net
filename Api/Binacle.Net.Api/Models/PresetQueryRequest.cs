using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text.Json;

namespace Binacle.Net.Api.Models
{
    public class PresetQueryRequest : IWithSwaggerExample
    {
        public List<Box> Items { get; set; }

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


    // Swashbuckle.AspNetCore.Filters
    //public class PresetQueryRequestExampleProvider : IExamplesProvider<PresetQueryRequest>
    //{
    //    public PresetQueryRequest GetExamples()
    //    {
    //        return PresetQueryRequest.Example;
    //    }
    //}

    public class PresetQueryRequestSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type == typeof(PresetQueryRequest))
            {
                var items = new OpenApiArray();
                items.Add(new OpenApiObject()
                {
                    ["id"] = new OpenApiString("box_1"),
                    ["quantity"] = new OpenApiInteger(2),
                    ["length"] = new OpenApiInteger(2),
                    ["width"] = new OpenApiInteger(5),
                    ["height"] = new OpenApiInteger(10),
                });

                items.Add(new OpenApiObject()
                {
                    ["id"] = new OpenApiString("box_2"),
                    ["quantity"] = new OpenApiInteger(1),
                    ["length"] = new OpenApiInteger(12),
                    ["width"] = new OpenApiInteger(15),
                    ["height"] = new OpenApiInteger(10),
                });

                items.Add(new OpenApiObject()
                {
                    ["id"] = new OpenApiString("box_3"),
                    ["quantity"] = new OpenApiInteger(1),
                    ["length"] = new OpenApiInteger(12),
                    ["width"] = new OpenApiInteger(15),
                    ["height"] = new OpenApiInteger(10),
                });

                schema.Example = new OpenApiObject()
                {
                    ["items"] = items
                };
            }
        }
    }
}
