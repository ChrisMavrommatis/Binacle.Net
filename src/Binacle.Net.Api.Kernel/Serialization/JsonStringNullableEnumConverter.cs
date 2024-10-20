using System.Text.Json;
using System.Text.Json.Serialization;

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.

namespace Binacle.Net.Api.Kernel.Serialization;


public class JsonStringNullableEnumConverter<T> : JsonConverter<T>
{
	private readonly Type _underlyingType;

	public JsonStringNullableEnumConverter()
	{
		_underlyingType = Nullable.GetUnderlyingType(typeof(T))!;
	}

	public override bool CanConvert(Type typeToConvert)
	{
		return typeof(T).IsAssignableFrom(typeToConvert);
	}

	public override T Read(ref Utf8JsonReader reader,
		Type typeToConvert,
		JsonSerializerOptions options)
	{
		string value = reader.GetString()!;
		if (String.IsNullOrEmpty(value)) return default!;

		// for performance, parse with ignoreCase:false first.
		if (!Enum.TryParse(_underlyingType, value, ignoreCase: false, out object result) &&
			!Enum.TryParse(_underlyingType, value, ignoreCase: true, out result))
		{
			return default!;
		}
		return (T)result;
	}

	public override void Write(Utf8JsonWriter writer,
		T value,
		JsonSerializerOptions options)
	{
		writer.WriteStringValue(value?.ToString());
	}
}

