using System.Buffers.Binary;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Binacle.ViPaq;

// Writes one value at a time, little-endian (PROTOCOL.md §0). It does not know what a dimension or a
// coordinate is: grouping values into triples, and the order they go in, is the caller's business — the layout
// codecs for the items, ProtocolEncoder for the bin.
internal class ProtocolWriter<T> : IDisposable, IAsyncDisposable
	where T : struct, IBinaryInteger<T>
{
	private readonly Stream stream;
	private readonly bool isMemoryStream;
	private bool disposed;

	public ProtocolWriter(Stream output)
	{
		this.stream = output;
		this.isMemoryStream = stream.GetType() == typeof(MemoryStream);
		this.disposed = false;
	}

	// The item count is always a uint16, whatever the widths say (PROTOCOL.md §3).
	public void WriteUInt16(ushort value)
	{
		Span<byte> buffer = stackalloc byte[sizeof(ushort)];
		BinaryPrimitives.WriteUInt16LittleEndian(buffer, value);
		this.InternalWrite(buffer);
	}

	// Picks the write for a Width. Write8Bits / Write16Bits are the two it can pick — the names match the
	// Width enum, and each narrows the caller's T down to the wire width before writing those bytes.
	public void WriteValue(T value, Width width)
	{
		switch (width)
		{
			case Width.Eight:
				this.Write8Bits(value);
				break;

			case Width.Sixteen:
				this.Write16Bits(value);
				break;

			default:
				throw new ArgumentOutOfRangeException(nameof(width), width, $"Width {width} is not supported");
		}
	}

	public void Write8Bits(T value)
	{
		this.InternalWriteByte(byte.CreateChecked(value));
	}

	public void Write16Bits(T value)
	{
		Span<byte> buffer = stackalloc byte[sizeof(ushort)];
		BinaryPrimitives.WriteUInt16LittleEndian(buffer, ushort.CreateChecked(value));
		this.InternalWrite(buffer);
	}

	private void InternalWriteByte(byte value)
	{
		if (this.isMemoryStream)
		{
			Unsafe.As<MemoryStream>(this.stream).WriteByte(value);
			return;
		}

		this.ThrowIfDisposed();
		this.stream.WriteByte(value);
	}

	private void InternalWrite(Span<byte> buffer)
	{
		if (this.isMemoryStream)
		{
			Unsafe.As<MemoryStream>(this.stream).Write(buffer);
		}
		else
		{
			this.ThrowIfDisposed();
			this.stream.Write(buffer);
		}
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
		// Flush has to stay inside the guard. Calling it on an already-disposed stream throws on
		// non-MemoryStream streams, so a second Dispose would blow up if Flush ran first.
		if (!this.disposed)
		{
			this.stream.Flush();
			this.stream.Dispose();
			this.disposed = true;
		}
	}

	public async ValueTask DisposeAsync()
	{
		// Same as Dispose: flush only while we still own a live stream, so a second call is a no-op.
		if (!this.disposed)
		{
			await this.stream.FlushAsync();
			await this.stream.DisposeAsync();
			this.disposed = true;
		}
	}
}
