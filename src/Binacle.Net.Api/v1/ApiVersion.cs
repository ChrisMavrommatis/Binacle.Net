using Asp.Versioning.ApiExplorer;
using Swashbuckle.AspNetCore.SwaggerGen;
using ChrisMavrommatis.Swashbuckle;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Binacle.Net.Api.v1;

internal class ApiVersion : IApiVersion
{
	public const string Number = "1.0";
	public const bool IsDeprecated = true;
	public const bool IsExperimental = false;

	public int MajorNumber => int.Parse(Number.Split('.')[0]);
	public bool Deprecated => IsDeprecated;
	public bool Experimental => IsExperimental;
	
	public void ConfigureSwaggerOptions(SwaggerGenOptions options, IApiVersionDescriptionProvider provider)
	{
		options.AddPolymorphicTypeMappings(_polymorphicTypeMappings);
		
		var description = provider.ApiVersionDescriptions
			.FirstOrDefault(x => x.ApiVersion.MajorVersion == this.MajorNumber)!;

		var info = ApiDocument.CreateApiInfo(this, description);
	
		options.SwaggerDoc(description.GroupName, info);
	}

	public void ConfigureSwaggerUI(SwaggerUIOptions options, IApiVersionDescriptionProvider provider)
	{
		var description = provider.ApiVersionDescriptions
			.FirstOrDefault(x => x.ApiVersion.MajorVersion == this.MajorNumber);
		
		options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", $"Binacle.Net API {description.GroupName}");
	}
	
	internal static Dictionary<Type, Type[]> _polymorphicTypeMappings = new()
	{
		{ typeof(v1.Models.Errors.IApiError), [ 
				typeof(v1.Models.Errors.FieldValidationError), 
				typeof(v1.Models.Errors.ParameterError), 
				typeof(v1.Models.Errors.ExceptionError)
			]
		}
	};

}

