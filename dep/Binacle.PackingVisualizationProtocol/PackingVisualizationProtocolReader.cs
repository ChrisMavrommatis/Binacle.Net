using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace Binacle.PackingVisualizationProtocol;

public class PackingVisualizationProtocolReader : IDisposable, IAsyncDisposable
{
	private readonly Stream _stream;
	private readonly bool _isLittleIndian;
	private readonly bool _isMemoryStream;
	private bool _disposed;

	public PackingVisualizationProtocolReader(Stream input) : this(input, BitConverter.IsLittleEndian)
	{
		
	}
	
	public PackingVisualizationProtocolReader(Stream input, bool isLittleIndian)
	{
		_stream = input;
		_isLittleIndian = isLittleIndian;
		_isMemoryStream = _stream.GetType() == typeof(MemoryStream);
		_disposed = false;
	}

	public byte ReadByte()
	{
		return (byte)_stream.ReadByte();
	}

	public sbyte ReadSByte()
	{
		return (sbyte)_stream.ReadByte();
	}

	public ushort ReadUInt16()
	{
		var buffer = InternalRead(stackalloc byte[sizeof(ushort)]);
		
		if (_isLittleIndian)
		{
			return BinaryPrimitives.ReadUInt16LittleEndian(buffer);
		}

		return BinaryPrimitives.ReadUInt16BigEndian(buffer);
	}

	public short ReadInt16()
	{
		var buffer = InternalRead(stackalloc byte[sizeof(short)]);

		if (_isLittleIndian)
		{
			return BinaryPrimitives.ReadInt16LittleEndian(buffer);
		}

		return BinaryPrimitives.ReadInt16BigEndian(buffer);
	}

	public uint ReadUInt32()
	{
		var buffer = InternalRead(stackalloc byte[sizeof(uint)]);
		
		if (_isLittleIndian)
		{
			return BinaryPrimitives.ReadUInt32LittleEndian(buffer);
		}

		return BinaryPrimitives.ReadUInt32BigEndian(buffer);
	}

	public int ReadInt32()
	{
		var buffer = InternalRead(stackalloc byte[sizeof(int)]);

		if (_isLittleIndian)
		{
			return BinaryPrimitives.ReadInt32LittleEndian(buffer);
		}

		return BinaryPrimitives.ReadInt32BigEndian(buffer);
	}

	public ulong ReadUInt64()
	{
		var buffer = InternalRead(stackalloc byte[sizeof(ulong)]);
		
		if (_isLittleIndian)
		{
			return BinaryPrimitives.ReadUInt64LittleEndian(buffer);
		}

		return BinaryPrimitives.ReadUInt64BigEndian(buffer);
	}

	public long ReadInt64()
	{
		var buffer = InternalRead(stackalloc byte[sizeof(long)]);
		
		if (_isLittleIndian)
		{
			return BinaryPrimitives.ReadInt64LittleEndian(buffer);
		}

		return BinaryPrimitives.ReadInt64BigEndian(buffer);
	}

	public float ReadSingle()
	{
		var buffer = InternalRead(stackalloc byte[sizeof(float)]);
		
		if (_isLittleIndian)
		{
			return BinaryPrimitives.ReadSingleLittleEndian(buffer);
		}

		return BinaryPrimitives.ReadSingleBigEndian(buffer);
	}

	public double ReadDouble()
	{
		var buffer = InternalRead(stackalloc byte[sizeof(double)]);
		
		if (_isLittleIndian)
		{
			return BinaryPrimitives.ReadDoubleLittleEndian(buffer);
		}

		return BinaryPrimitives.ReadDoubleBigEndian(buffer);
	}

	public decimal ReadDecimal()
	{
		var buffer = InternalRead(stackalloc byte[sizeof(decimal)]);
		
		Span<int> bits = stackalloc int[4];
		if (_isLittleIndian)
		{
			bits[0] = BinaryPrimitives.ReadInt32LittleEndian(buffer);
			bits[1] = BinaryPrimitives.ReadInt32LittleEndian(buffer);
			bits[2] = BinaryPrimitives.ReadInt32LittleEndian(buffer);
			bits[3] = BinaryPrimitives.ReadInt32LittleEndian(buffer);
		}
		else
		{
			bits[0] = BinaryPrimitives.ReadInt32BigEndian(buffer);
			bits[1] = BinaryPrimitives.ReadInt32BigEndian(buffer);
			bits[2] = BinaryPrimitives.ReadInt32BigEndian(buffer);
			bits[3] = BinaryPrimitives.ReadInt32BigEndian(buffer);
		}

		return new decimal(bits);
	}

	private byte InternalReadByte()
	{
		ThrowIfDisposed();

		int b = _stream.ReadByte();
		if (b == -1)
		{
			throw new EndOfStreamException("Cannot read beyond the end of the stream");
		}

		return (byte)b;
	}

	private ReadOnlySpan<byte> InternalRead(Span<byte> buffer)
	{
		if (buffer.Length == 1)
		{
			return new byte[] { ReadByte() };
		}

		if (_isMemoryStream)
		{
			Unsafe.As<MemoryStream>(_stream).ReadExactly(buffer);
		}
		else
		{
			ThrowIfDisposed();
			_stream.ReadExactly(buffer);
		}

		return buffer;
	}

	private void ThrowIfDisposed()
	{
		if (_disposed)
		{
			throw new ObjectDisposedException(null, "Stream has been disposed");
		}
	}

	public void Dispose()
	{
		if (!_disposed)
		{
			_stream.Dispose();
			_disposed = true;
		}
	}

	public async ValueTask DisposeAsync()
	{
		if (!_disposed)
		{
			await _stream.DisposeAsync();
			_disposed = true;
		}
	}
}
