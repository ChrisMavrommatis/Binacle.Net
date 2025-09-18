using System.Text.Json;

namespace Binacle.Net.ServiceModule.Infrastructure.Common.Models;

internal class FileHashStore
{
	private static readonly JsonSerializerOptions _defaultSerializerOptions = new()
	{
		PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
		WriteIndented = false,
	};
	
	private readonly JsonSerializerOptions jsonSerializerOptions;
	private readonly Dictionary<string, Entry> entries = new();
	
	internal FileHashStore() : this(_defaultSerializerOptions)
	{
		
	}
	internal FileHashStore(JsonSerializerOptions jsonSerializerOptions)
	{
		if (jsonSerializerOptions is null)
		{
			throw new ArgumentNullException(nameof(jsonSerializerOptions));
		}

		if (jsonSerializerOptions.WriteIndented)
		{
			throw new ArgumentException("WriteIndented in JsonSerializerOptions must be false, as the file is ndjson");
		}

		this.jsonSerializerOptions = jsonSerializerOptions;
	}

	public async Task LoadAsync(string filePath)
	{
		if(!File.Exists(filePath))
			return;
		
		var lines = await File.ReadAllLinesAsync(filePath);
		foreach (var line in lines)
		{
			var entry = JsonSerializer.Deserialize<Entry>(line, this.jsonSerializerOptions);
			if (entry is not null)
			{
				this.entries[entry.FilePath] = entry;
			}
		}
	}
	public async Task SaveAsync(string filePath)
	{
		var lines = this.entries.Values
			.Select(entry => JsonSerializer.Serialize(entry, this.jsonSerializerOptions));
		await File.WriteAllLinesAsync(filePath, lines);
	}
	public bool HashMatches(string filePath, string md5Hash)
	{
		return this.entries.TryGetValue(filePath, out var entry) && entry.Md5Hash == md5Hash;
	}
	
	public void Set(string file, string md5)
	{
		this.entries[file] = new Entry
		{
			FilePath = file,
			Md5Hash = md5
		};
	}

	private class Entry
	{
		public string FilePath { get; set; } = null!;
		public string Md5Hash { get; set; } = null!;
	}

	
}
