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
