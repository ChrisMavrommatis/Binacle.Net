namespace Binacle.Lib.Abstractions.Models;

public interface IWithID : IWithReadOnlyID
{
	string ID { get; set; }
}
