using Microsoft.AspNetCore.Http;

namespace Binacle.Net;

// Paths the API serves that must never answer with a web page. Whoever maps a path adds it here, because
// some of them are configurable and only the module that owns one knows where it ended up.
public class ReservedPathOptions
{
	private readonly HashSet<string> prefixes = new(StringComparer.OrdinalIgnoreCase);

	public void AddPrefix(string prefix)
	{
		if (!string.IsNullOrWhiteSpace(prefix))
		{
			this.prefixes.Add(prefix);
		}
	}

	public bool Covers(PathString path)
	{
		foreach (var prefix in this.prefixes)
		{
			if (path.StartsWithSegments(prefix))
				return true;
		}

		return false;
	}

	public IReadOnlyCollection<string> Prefixes => this.prefixes;
}
