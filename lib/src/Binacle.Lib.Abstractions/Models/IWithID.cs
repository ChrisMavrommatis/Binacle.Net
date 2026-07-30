namespace Binacle.Lib.Abstractions.Models;

public interface IWithID : IWithReadOnlyID
{
	new string ID { get; set; }
}
