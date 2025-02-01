using System.Buffers.Binary;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Binacle.ViPaq;

internal class ProtocolReader<T> : IDisposable, IAsyncDisposable
	where T : struct, IBinaryInteger<T>, INumber<T>, IComparable<T>
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
	
	public byte ReadByte()
	{
		return (byte)this.InternalReadByte();
	}

	public T ReadAsByte()
	{
		return (T)(object)this.InternalReadByte();
	}
	
	public ushort ReadUInt16()
	{
		var buffer = InternalReadBuffer(stackalloc byte[sizeof(ushort)]);
		return BinaryPrimitives.ReadUInt16LittleEndian(buffer);
	}

	public T ReadAsUInt16()
	{
		var buffer = InternalReadBuffer(stackalloc byte[sizeof(ushort)]);
		var ushortValue = BinaryPrimitives.ReadUInt16LittleEndian(buffer);
		return T.CreateChecked(ushortValue);
	}
	
	public uint ReadUInt32()
	{
		var buffer = InternalReadBuffer(stackalloc byte[sizeof(uint)]);
		return BinaryPrimitives.ReadUInt32LittleEndian(buffer);
	}

	public T ReadAsUInt32()
	{
		var buffer = InternalReadBuffer(stackalloc byte[sizeof(uint)]);
		var uintValue = BinaryPrimitives.ReadUInt32LittleEndian(buffer);
		return T.CreateChecked(uintValue);
	}
	
	public ulong ReadUInt64()
	{
		var buffer = InternalReadBuffer(stackalloc byte[sizeof(ulong)]);
		return BinaryPrimitives.ReadUInt64LittleEndian(buffer);
	}

	public T ReadAsUInt64()
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
