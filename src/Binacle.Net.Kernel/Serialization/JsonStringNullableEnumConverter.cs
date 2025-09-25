using System.Text.Json;
using System.Text.Json.Serialization;

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.

namespace Binacle.Net.Kernel.Serialization;

public class JsonStringNullableEnumConverter<T> : JsonConverter<T>
{
	private readonly Type underlyingType;

	public JsonStringNullableEnumConverter()
	{
		this.underlyingType = Nullable.GetUnderlyingType(typeof(T))!;
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
		if (!Enum.TryParse(this.underlyingType, value, ignoreCase: false, out object result) &&
		    !Enum.TryParse(this.underlyingType, value, ignoreCase: true, out result))
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

public class JsonStringNullableEnumConverter : JsonConverterFactory
{
	public override bool CanConvert(Type typeToConvert)
	{
		var underlyingType = Nullable.GetUnderlyingType(typeToConvert);
		if (underlyingType is null)
		{
			return false;
		}
		return underlyingType.IsEnum;
	}

	public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
	{
		Type converterType = typeof(NullableEnumConverter<>).MakeGenericType(typeToConvert);
		return (JsonConverter)Activator.CreateInstance(converterType)!;
	}
}

internal sealed class NullableEnumConverter<T> : JsonConverter<T>
{
	private readonly Type underlyingType;

	public NullableEnumConverter()
	{
		this.underlyingType = Nullable.GetUnderlyingType(typeof(T))!;
	}

	public override T ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (!this.TryParseEnumFromString(ref reader, out T result))
		{
			throw new JsonException($"Could not read property for {typeof(T).Name}");
		}

		return result;
	}

	public override void WriteAsPropertyName(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
	{
		writer.WriteStringValue(this.WriteValue(value));
	}

	public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		return this.TryParseEnumFromString(ref reader, out T result) ? result : default!;
	}

	public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
	{
		writer.WriteStringValue(this.WriteValue(value));
	}

	private ReadOnlySpan<char> WriteValue(T value)
	{
		return value?.ToString();
	}

	private bool TryParseEnumFromString(ref Utf8JsonReader reader, out T result)
	{
		if (reader.TokenType == JsonTokenType.Null)
		{
			result = default!;
			return true;
		}
		string value = reader.GetString()!;

		if (String.IsNullOrEmpty(value))
		{
			result = default!;
			return true;
		}
		if (Enum.TryParse(this.underlyingType, value, ignoreCase: false, out object parsedResult))
		{
			result = (T)parsedResult;
			return true;
		}
		if (Enum.TryParse(this.underlyingType, value, ignoreCase: true, out object parsedResult2))
		{
			result = (T)parsedResult2;
			return true;
		}

		result = default!;
		return true;
	}
}
