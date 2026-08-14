using System.Buffers.Binary;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Binacle.ViPaq;

// Reads one value at a time, little-endian. Which values come back in what order is the caller's business.
//
// Nothing is range-checked: an 8- or 16-bit field cannot hold a value outside ViPaq's range, which is what
// PROTOCOL.md §5 means by "a decoder has nothing to range-check".
internal class ProtocolReader<T> : IDisposable, IAsyncDisposable
	where T : struct, IBinaryInteger<T>
{
	private readonly Stream stream;
	private readonly bool isMemoryStream;
	private bool disposed;

	public ProtocolReader(Stream input)
	{
		this.stream = input;
		this.isMemoryStream = stream.GetType() == typeof(MemoryStream);
		this.disposed = false;
	}

	// The item count is always a uint16, whatever the widths say (PROTOCOL.md §3).
	public ushort ReadUInt16()
	{
		var buffer = this.InternalReadBuffer(stackalloc byte[sizeof(ushort)]);
		return BinaryPrimitives.ReadUInt16LittleEndian(buffer);
	}

	// Picks the read for a Width. Read8Bits / Read16Bits read the wire width, then widen to the caller's T.
	public T ReadValue(Width width)
	{
		return width switch
		{
			Width.Eight => this.Read8Bits(),
			Width.Sixteen => this.Read16Bits(),
			_ => throw new ArgumentOutOfRangeException(nameof(width), width, $"Width {width} is not supported")
		};
	}

	public T Read8Bits()
	{
		return T.CreateChecked(this.InternalReadByte());
	}

	public T Read16Bits()
	{
		var buffer = this.InternalReadBuffer(stackalloc byte[sizeof(ushort)]);
		return T.CreateChecked(BinaryPrimitives.ReadUInt16LittleEndian(buffer));
	}

	private byte InternalReadByte()
	{
		var read = this.isMemoryStream
			? Unsafe.As<MemoryStream>(this.stream).ReadByte()
			: ReadByteChecked();

		if (read < 0)
		{
			// EOF. Reject rather than return a phantom value: a truncated body must fail (PROTOCOL.md §7).
			throw new EndOfStreamException("Unexpected end of stream while reading a byte.");
		}

		return (byte)read;

		int ReadByteChecked()
		{
			this.ThrowIfDisposed();
			return this.stream.ReadByte();
		}
	}

	private ReadOnlySpan<byte> InternalReadBuffer(Span<byte> buffer)
	{
		if (this.isMemoryStream)
		{
			Unsafe.As<MemoryStream>(this.stream).ReadExactly(buffer);
		}
		else
		{
			this.ThrowIfDisposed();
			this.stream.ReadExactly(buffer);
		}

		return buffer;
	}

	private void ThrowIfDisposed()
	{
		if (this.disposed)
		{
			throw new ObjectDisposedException(null, "Stream has been disposed");
		}
	}

	public void Dispose()
	{
		if (!this.disposed)
		{
			this.stream.Dispose();
			this.disposed = true;
		}
	}

	public async ValueTask DisposeAsync()
	{
		if (!this.disposed)
		{
			await this.stream.DisposeAsync();
			this.disposed = true;
		}
	}
}
