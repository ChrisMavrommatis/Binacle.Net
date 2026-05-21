namespace Binacle.Net.Kernel.Configuration.Models;

public class ConnectionString
{
	private readonly string value;
	private readonly Dictionary<string, string> keyValuePairs;
	internal ConnectionString(string value)
	{
		this.value = value;
		var parts = this.value.Split(";", StringSplitOptions.RemoveEmptyEntries);
		this.keyValuePairs = new Dictionary<string, string>();

		foreach (var part in parts)
		{
			var keyValue = part.Split("=");
			this.keyValuePairs.Add(keyValue[0], keyValue[1]);
		}
	}

	public string? Get(string key)
	{
		if(this.keyValuePairs.TryGetValue(key, out var value))
		{
			return value;
		}

		return null;
	}
	
	public string GetOrThrow(string key)
	{
		if(!this.keyValuePairs.TryGetValue(key, out var value) || string.IsNullOrWhiteSpace(value))
		{
			throw new KeyNotFoundException($"Key '{key}' not found in connection string");
		}
		return value;
	}
	
	public string GetOrDefault(string key, string defaultValue)
	{
		return this.keyValuePairs.GetValueOrDefault(key, defaultValue);
	}
	
	
	public T GetOrDefault<T>(string key, T defaultValue)
	{
		if(!this.keyValuePairs.TryGetValue(key, out var value))
		{
			return defaultValue;
		}


		T? returnValue = default;
		switch (typeof(T))
		{
			case var t when t == typeof(int):
				if(int.TryParse(value, out var intValue))
				{
					returnValue = (T)(object)intValue;
				}
				break;
			case var t when t == typeof(bool):
				if(bool.TryParse(value, out var boolValue))
				{
					returnValue = (T)(object)boolValue;
				}
				break;
			case var t when t == typeof(double):
				if(double.TryParse(value, out var doubleValue))
				{
					returnValue = (T)(object)doubleValue;
				}
				break;
			case var t when t == typeof(Guid):
				if(Guid.TryParse(value, out var guidValue))
				{
					returnValue = (T)(object)guidValue;
				}
				break;
			case var t when t.IsEnum:
				try
				{
					returnValue = (T)Enum.Parse(t, value, true);
				}
				catch
				{
					// ignore
				}
				break;
			default:
				throw new NotSupportedException($"Type '{typeof(T)}' is not supported");
			
		}
		
		return returnValue ?? defaultValue;
	}

	public bool Has(string key)
	{
		if(this.keyValuePairs.TryGetValue(key, out var value))
		{
			return !string.IsNullOrWhiteSpace(value);
		}
		return false;
	}
	
	public ConnectionString ThrowIfNotExists(string key)
	{
		if(this.keyValuePairs.TryGetValue(key, out var value))
		{
			if(!string.IsNullOrWhiteSpace(value))
			{
				return this;
			}
		}
		throw new KeyNotFoundException($"Key '{key}' not found in connection string");
	}


	// implicit assign
	public static implicit operator string(ConnectionString connectionString) => connectionString.value;
}
