namespace Binacle.ViPaq.UnitTests;

// The writer and reader reject bad use rather than corrupting the stream.
[Trait("Behavioral Tests", "Ensures operations behave as expected")]
public class ProtocolBehaviorTests
{
	[Fact]
	public void Write8Bits_Throws_When_Value_Exceeds_Byte()
	{
		using var stream = new MemoryStream();
		var writer = new ProtocolWriter<int>(stream);

		Should.Throw<OverflowException>(() => writer.Write8Bits(256));
	}

	[Fact]
	public void Write16Bits_Throws_When_Value_Exceeds_UInt16()
	{
		using var stream = new MemoryStream();
		var writer = new ProtocolWriter<int>(stream);

		Should.Throw<OverflowException>(() => writer.Write16Bits(70_000));
	}

	// MemoryStream skips the disposed check, so these wrap it in a BufferedStream to reach that path.
	[Fact]
	public void Writer_Throws_When_Used_After_Dispose()
	{
		var writer = new ProtocolWriter<int>(new BufferedStream(new MemoryStream()));
		writer.Dispose();

		Should.Throw<ObjectDisposedException>(() => writer.Write8Bits(0x01));
	}

	[Fact]
	public void Reader_Throws_When_Used_After_Dispose()
	{
		var reader = new ProtocolReader<int>(new BufferedStream(new MemoryStream([1, 2, 3, 4])));
		reader.Dispose();

		Should.Throw<ObjectDisposedException>(() => reader.Read8Bits());
	}

	// BufferedStream, not MemoryStream: MemoryStream.Flush() is a no-op even after close and would hide a
	// second Dispose flushing a dead stream. BufferedStream.Flush() throws once disposed.
	[Fact]
	public void Writer_Dispose_Is_Safe_To_Call_Twice()
	{
		var writer = new ProtocolWriter<int>(new BufferedStream(new MemoryStream()));
		writer.Dispose();

		Should.NotThrow(() => writer.Dispose());
	}

	[Fact]
	public async Task Writer_DisposeAsync_Is_Safe_To_Call_Twice()
	{
		var writer = new ProtocolWriter<int>(new BufferedStream(new MemoryStream()));
		await writer.DisposeAsync();

		await Should.NotThrowAsync(async () => await writer.DisposeAsync());
	}

	[Fact]
	public void Reader_Dispose_Is_Safe_To_Call_Twice()
	{
		var reader = new ProtocolReader<int>(new MemoryStream([1, 2, 3, 4]));
		reader.Dispose();

		Should.NotThrow(() => reader.Dispose());
	}
}
