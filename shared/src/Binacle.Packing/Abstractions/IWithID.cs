namespace Binacle.Packing;

public interface IWithID : IWithReadOnlyID
{
	new string ID { get; set; }
}
