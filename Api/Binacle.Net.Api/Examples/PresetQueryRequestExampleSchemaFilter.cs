using Binacle.Net.Api.Models.Requests;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Binacle.Net.Api.Examples;

public class PresetQueryRequestExampleSchemaFilter : ISchemaFilter
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
