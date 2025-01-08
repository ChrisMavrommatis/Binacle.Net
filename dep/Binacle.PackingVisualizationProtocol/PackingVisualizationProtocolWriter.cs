using System.Buffers.Binary;

namespace Binacle.PackingVisualizationProtocol;

public class PackingVisualizationProtocolWriter :  IDisposable, IAsyncDisposable
{
	private Stream _stream;
	private readonly bool _isLittleIndian;

	protected PackingVisualizationProtocolWriter()
	{
		_stream = Stream.Null;
		_isLittleIndian = BitConverter.IsLittleEndian;
	}

	public PackingVisualizationProtocolWriter(Stream output) : this(output, BitConverter.IsLittleEndian)
	{
		
	} 
	
	public PackingVisualizationProtocolWriter(Stream output, bool isLittleIndian) 
	{
		_stream = output;
		_isLittleIndian = isLittleIndian;
	}
	
	public void WriteByte(byte value)
	{
		_stream.WriteByte(value);
	}
	
	public void WriteSByte(sbyte value)
	{
		_stream.WriteByte((byte)value);
	}


	
	public void WriteUInt16(ushort value)
	{
		Span<byte> buffer = stackalloc byte[sizeof(ushort)];
		if (_isLittleIndian)
		{
			BinaryPrimitives.WriteUInt16LittleEndian(buffer, value);
		}
		else
		{
			BinaryPrimitives.WriteUInt16BigEndian(buffer, value);
		}
		_stream.Write(buffer);
	}	
	
	public void WriteInt16(short value)
	{
		Span<byte> buffer = stackalloc byte[sizeof(short)];
		if (_isLittleIndian)
		{
			BinaryPrimitives.WriteInt16LittleEndian(buffer, value);
		}
		else
		{
			BinaryPrimitives.WriteInt16BigEndian(buffer, value);
		}
		_stream.Write(buffer);
	}
	
	public void WriteUInt32(uint value)
	{
		Span<byte> buffer = stackalloc byte[sizeof(uint)];
		if (_isLittleIndian)
		{
			BinaryPrimitives.WriteUInt32LittleEndian(buffer, value);
		}
		else
		{
			BinaryPrimitives.WriteUInt32BigEndian(buffer, value);
		}
		_stream.Write(buffer);
	}
	
	public void WriteInt32(int value)
	{
		Span<byte> buffer = stackalloc byte[sizeof(int)];
		if (_isLittleIndian)
		{
			BinaryPrimitives.WriteInt32LittleEndian(buffer, value);
		}
		else
		{
			BinaryPrimitives.WriteInt32BigEndian(buffer, value);
		}
		_stream.Write(buffer);
	}
	
	public void WriteUInt64(ulong value)
	{
		Span<byte> buffer = stackalloc byte[sizeof(ulong)];
		if (_isLittleIndian)
		{
			BinaryPrimitives.WriteUInt64LittleEndian(buffer, value);
		}
		else
		{
			BinaryPrimitives.WriteUInt64BigEndian(buffer, value);
		}
		_stream.Write(buffer);
	}
	
	public void WriteInt64(long value)
	{
		Span<byte> buffer = stackalloc byte[sizeof(long)];
		if (_isLittleIndian)
		{
			BinaryPrimitives.WriteInt64LittleEndian(buffer, value);
		}
		else
		{
			BinaryPrimitives.WriteInt64BigEndian(buffer, value);
		}
		_stream.Write(buffer);
	}
	
	public void WriteSingle(float value)
	{
		Span<byte> buffer = stackalloc byte[sizeof(float)];
		if (_isLittleIndian)
		{
			BinaryPrimitives.WriteSingleLittleEndian(buffer, value);
		}
		else
		{
			BinaryPrimitives.WriteSingleBigEndian(buffer, value);
		}
		_stream.Write(buffer);
	}
	
	public void WriteDouble(double value)
	{
		Span<byte> buffer = stackalloc byte[sizeof(double)];
		if (_isLittleIndian)
		{
			BinaryPrimitives.WriteDoubleLittleEndian(buffer, value);
		}
		else
		{
			BinaryPrimitives.WriteDoubleBigEndian(buffer, value);
		}
		_stream.Write(buffer);
	}
	
	public void WriteDecimal(decimal value)
	{
		Span<byte> buffer = stackalloc byte[sizeof(decimal)];
		Span<int> bits = stackalloc int[4];
		decimal.GetBits(value, bits);

		var low = bits[0];
		var mid = bits[1];
		var high = bits[2];
		var flags = bits[3];
		
		if (_isLittleIndian)
		{
			BinaryPrimitives.WriteInt32LittleEndian(buffer, low);
			BinaryPrimitives.WriteInt32LittleEndian(buffer, mid);
			BinaryPrimitives.WriteInt32LittleEndian(buffer, high);
			BinaryPrimitives.WriteInt32LittleEndian(buffer, flags);
		}
		else
		{
			BinaryPrimitives.WriteInt32BigEndian(buffer, low);
			BinaryPrimitives.WriteInt32BigEndian(buffer, mid);
			BinaryPrimitives.WriteInt32BigEndian(buffer, high);
			BinaryPrimitives.WriteInt32BigEndian(buffer, flags);
		}
		
		_stream.Write(buffer);
	}

	public void Dispose()
	{
		_stream.Flush();
		_stream.Dispose();
	}

	public async ValueTask DisposeAsync()
	{
		await _stream.FlushAsync();		
		await _stream.DisposeAsync();
	}
}
