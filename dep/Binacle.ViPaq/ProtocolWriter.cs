using System.Buffers.Binary;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Binacle.ViPaq.Abstractions;
using Binacle.ViPaq.Models;

namespace Binacle.ViPaq;

internal class ProtocolWriter<T> : IDisposable, IAsyncDisposable
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
	
	public void WriteByte(byte value)
	{
		this.InternalWriteByte(value);
	}

	public void WriteAsByte(T value)
	{
		// convertTo Byte
		var byteValue = byte.CreateChecked(value);
		this.InternalWriteByte(byteValue);
	}

	public void WriteUInt16(ushort value)
	{
		Span<byte> buffer = stackalloc byte[sizeof(ushort)];
		BinaryPrimitives.WriteUInt16LittleEndian(buffer, value);
		InternalWrite(buffer);
	}
	
	public void WriteAsUInt16(T value)
	{
		Span<byte> buffer = stackalloc byte[sizeof(ushort)];
		var ushortValue = ushort.CreateChecked(value);
		BinaryPrimitives.WriteUInt16LittleEndian(buffer, ushortValue);
		InternalWrite(buffer);
	}
	
	public void WriteUInt32(uint value)
	{
		Span<byte> buffer = stackalloc byte[sizeof(uint)];
		BinaryPrimitives.WriteUInt32LittleEndian(buffer, value);
		InternalWrite(buffer);
	}

	public void WriteAsUInt32(T value)
	{
		Span<byte> buffer = stackalloc byte[sizeof(uint)];
		var uintValue = uint.CreateChecked(value);
		BinaryPrimitives.WriteUInt32LittleEndian(buffer, uintValue);
		InternalWrite(buffer);
	}
	
	public void WriteUInt64(ulong value)
	{
		Span<byte> buffer = stackalloc byte[sizeof(ulong)];
		BinaryPrimitives.WriteUInt64LittleEndian(buffer, value);
		InternalWrite(buffer);
	}

	public void WriteAsUInt64(T value)
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
		this.stream.Flush();
		if (!this.disposed)
		{
			this.stream.Dispose();
			this.disposed = true;
		}
	}

	public async ValueTask DisposeAsync()
	{
		await this.stream.FlushAsync();
		if (!this.disposed)
		{
			await this.stream.DisposeAsync();
			this.disposed = true;
		}
	}
}
