using System.Buffers.Binary;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Binacle.ViPaq;

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

	// ReadByte / ReadUInt16 return a fixed wire width — used for the header (first byte, item count).
	// Read8Bits..Read64Bits read the same bytes but widen the value to T — used for dimensions and
	// coordinates, where the value type is the caller's T. The names match the BitSize enum.

	public byte ReadByte()
	{
		var read = this.InternalReadByte();
		if (read < 0)
		{
			// EOF: the stream had no more bytes. Reject instead of returning a phantom value — a
			// truncated body must fail, the same way the multi-byte ReadExactly path throws (PROTOCOL.md §7).
			throw new EndOfStreamException("Unexpected end of stream while reading a byte.");
		}
		return (byte)read;
	}

	public ushort ReadUInt16()
	{
		var buffer = InternalReadBuffer(stackalloc byte[sizeof(ushort)]);
		return BinaryPrimitives.ReadUInt16LittleEndian(buffer);
	}

	public T Read8Bits()
	{
		return T.CreateChecked(this.ReadByte());
	}

	public T Read16Bits()
	{
		var buffer = InternalReadBuffer(stackalloc byte[sizeof(ushort)]);
		var ushortValue = BinaryPrimitives.ReadUInt16LittleEndian(buffer);
		return T.CreateChecked(ushortValue);
	}

	public T Read32Bits()
	{
		var buffer = InternalReadBuffer(stackalloc byte[sizeof(uint)]);
		var uintValue = BinaryPrimitives.ReadUInt32LittleEndian(buffer);
		return T.CreateChecked(uintValue);
	}

	public T Read64Bits()
	{
		var buffer = InternalReadBuffer(stackalloc byte[sizeof(ulong)]);
		var ulongValue = BinaryPrimitives.ReadUInt64LittleEndian(buffer);
		return T.CreateChecked(ulongValue);
	}

	private int InternalReadByte()
	{
		if (this.isMemoryStream)
		{
			return Unsafe.As<MemoryStream>(this.stream).ReadByte();
		}

		ThrowIfDisposed();

		return this.stream.ReadByte();
	}

	private ReadOnlySpan<byte> InternalReadBuffer(Span<byte> buffer)
	{
		if (this.isMemoryStream)
		{
			Unsafe.As<MemoryStream>(this.stream).ReadExactly(buffer);
		}
		else
		{
			ThrowIfDisposed();
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
