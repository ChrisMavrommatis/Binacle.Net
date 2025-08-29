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

	// implicit assign
	public static implicit operator string(ConnectionString connectionString) => connectionString.value;
}
