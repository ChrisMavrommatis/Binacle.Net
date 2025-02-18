using Asp.Versioning.ApiExplorer;
using ChrisMavrommatis.Swashbuckle;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Binacle.Net.Api.v2;

internal class ApiVersion : IApiVersion
{
	public const string Number = "2.0";
	public const bool IsDeprecated = false;
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

		if (description is not null)
		{
			options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", $"Binacle.Net API {description.GroupName}");
		}
	}

	private static Dictionary<Type, Type[]> _polymorphicTypeMappings = new()
	{
		{ typeof(v2.Models.Errors.IApiError), [
				typeof(v2.Models.Errors.FieldValidationError), 
				typeof(v2.Models.Errors.ParameterError), 
				typeof(v2.Models.Errors.ExceptionError)
			] 
		}
	};

	
}
