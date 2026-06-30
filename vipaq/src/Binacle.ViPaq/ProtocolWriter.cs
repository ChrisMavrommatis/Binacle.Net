using System.Buffers.Binary;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Binacle.ViPaq.Abstractions;

namespace Binacle.ViPaq;

public class ProtocolWriter<T> : IDisposable, IAsyncDisposable
	where T : struct, IBinaryInteger<T>, INumber<T>, IComparable<T>
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

	// WriteByte / WriteUInt16 take a fixed wire width — used for the header (first byte, item count).
	// Write8Bits..Write64Bits narrow the caller's T down to the wire width, then write the same bytes —
	// used for dimensions and coordinates. The names match the BitSize enum.

	public void WriteByte(byte value)
	{
		this.InternalWriteByte(value);
	}

	public void WriteUInt16(ushort value)
	{
		Span<byte> buffer = stackalloc byte[sizeof(ushort)];
		BinaryPrimitives.WriteUInt16LittleEndian(buffer, value);
		InternalWrite(buffer);
	}

	public void Write8Bits(T value)
	{
		var byteValue = byte.CreateChecked(value);
		this.InternalWriteByte(byteValue);
	}

	public void Write16Bits(T value)
	{
		Span<byte> buffer = stackalloc byte[sizeof(ushort)];
		var ushortValue = ushort.CreateChecked(value);
		BinaryPrimitives.WriteUInt16LittleEndian(buffer, ushortValue);
		InternalWrite(buffer);
	}

	public void Write32Bits(T value)
	{
		Span<byte> buffer = stackalloc byte[sizeof(uint)];
		var uintValue = uint.CreateChecked(value);
		BinaryPrimitives.WriteUInt32LittleEndian(buffer, uintValue);
		InternalWrite(buffer);
	}

	public void Write64Bits(T value)
	{
		Span<byte> buffer = stackalloc byte[sizeof(ulong)];
		var ulongValue = ulong.CreateChecked(value);
		BinaryPrimitives.WriteUInt64LittleEndian(buffer, ulongValue);
		InternalWrite(buffer);
	}

	private void InternalWriteByte(byte value)
	{
		if (this.isMemoryStream)
		{
			Unsafe.As<MemoryStream>(this.stream).WriteByte(value);
			return;
		}

		ThrowIfDisposed();

		this.stream.WriteByte(value);
		return;
	}
	private void InternalWrite(Span<byte> buffer)
	{
		if (this.isMemoryStream)
		{
			Unsafe.As<MemoryStream>(this.stream).Write(buffer);
		}
		else
		{
			ThrowIfDisposed();
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
