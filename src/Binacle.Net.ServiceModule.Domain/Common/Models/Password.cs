namespace Binacle.Net.ServiceModule.Domain.Common.Models;

public class Password : ValueObject
{
	public string Type { get; }
	public string Hash { get; }
	public string? Salt { get; }

	public Password(string type, string hash, string? salt = null)
	{
		this.Type = type;
		this.Hash = hash;
		this.Salt = salt;
	}

	public static Password? TryParse(string? password)
	{
		if(string.IsNullOrWhiteSpace(password))
			return null;

		try
		{
			return Parse(password!);
		}
		catch (Exception)
		{
			return null;
		}
	}
	
	public static Password Parse(string password)
	{
		if (string.IsNullOrWhiteSpace(password))
			throw new ArgumentException("Password string cannot be null or empty.");

		int openBracket = password.IndexOf('[');
		int closeBracket = password.LastIndexOf(']');

		if (openBracket <= 0 || closeBracket <= openBracket + 1 || closeBracket != password.Length - 1)
			throw new ArgumentException("Invalid password format.");

		string type = password.Substring(0, openBracket);
		string inner = password.Substring(openBracket + 1, closeBracket - openBracket - 1);

		string hash;
		string? salt = null;

		int colonIndex = inner.IndexOf(':');
		if (colonIndex == -1)
		{
			// No salt
			hash = inner;
		}
		else
		{
			hash = inner.Substring(0, colonIndex);
			salt = inner.Substring(colonIndex + 1);
		}

		if (string.IsNullOrEmpty(type) || string.IsNullOrEmpty(hash))
			throw new ArgumentException("Type and hash cannot be empty.");

		return new Password(type, hash, salt);
	}

	public override string ToString()
	{
		if (string.IsNullOrEmpty(this.Salt))
		{
			return $"{this.Type}[{this.Hash}]";
		}

		return $"{this.Type}[{this.Hash}:{this.Salt}]";
	}

	protected override IEnumerable<object> GetAtomicValues()
	{
		yield return this.Type;
		yield return this.Hash;
		if (!string.IsNullOrEmpty(this.Salt))
		{
			yield return this.Salt;
		}
	}
}
