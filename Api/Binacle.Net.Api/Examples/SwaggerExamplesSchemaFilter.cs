using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections;
using System.Reflection;

namespace Binacle.Net.Api.Examples;

public class SwaggerExamplesSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        // check if type implements IWithSwaggerExample<>

        var typeWithSwaggerExample = context.Type.GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IWithSwaggerExample<>));

        if (typeWithSwaggerExample == null)
        {
            return;
        }

        // get THolder

        var holderType = typeWithSwaggerExample.GetGenericArguments().FirstOrDefault(x => typeof(ISwaggerExampleHolder).IsAssignableFrom(x));

        if (holderType == null)
        {
            return;
        }

        // get example declared field

        var exampleField = holderType.GetField(nameof(ISwaggerExampleHolder.Example), BindingFlags.Static | BindingFlags.Public);

        if (exampleField == null)
        {
            return;
        }

        // if field is not array or list create OpenApiObject

        IOpenApiAny example = null;

        if (!exampleField.FieldType.IsArray && !typeof(IList).IsAssignableFrom(exampleField.FieldType))
        {
        }

        if(example != null)
        {
            schema.Example = example;
        }
       
    }
}
