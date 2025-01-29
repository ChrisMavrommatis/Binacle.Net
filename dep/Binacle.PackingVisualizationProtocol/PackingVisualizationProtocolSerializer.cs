using System.IO.Compression;
using System.Numerics;
using System.Runtime.CompilerServices;
using Binacle.PackingVisualizationProtocol.Abstractions;
using Binacle.PackingVisualizationProtocol.Models;
using Version = Binacle.PackingVisualizationProtocol.Models.Version;

namespace Binacle.PackingVisualizationProtocol;

public static class PackingVisualizationProtocolSerializer
{
	private static Dictionary<Type, ByteSize> _byteSizes = new()
	{
		{typeof(byte), ByteSize.One},
		{typeof(sbyte), ByteSize.One},
		{typeof(ushort), ByteSize.Two},
		{typeof(short), ByteSize.Two},
		{typeof(uint), ByteSize.Four},
		{typeof(int), ByteSize.Four},
		{typeof(ulong), ByteSize.Eight},
		{typeof(long), ByteSize.Eight},
		
		{typeof(float), ByteSize.Four},
		{typeof(double), ByteSize.Eight}
	};

	private static Type[] _signedTypes = [
		typeof(sbyte),
		typeof(short),
		typeof(int),
		typeof(long),
		typeof(float),
		typeof(double),
	];
	
	private static Type[] _floatingTypes = [
		typeof(float),
		typeof(double),
	];
	
	private static EncodingInfo CreateEncodingInfo<T>()
		where T: struct, INumber<T>
	{
		var typeOfT = typeof(T);
		var byteSize = _byteSizes[typeOfT];
		var isFloating = _floatingTypes.Contains(typeOfT);
		var isSigned = _signedTypes.Contains(typeOfT);
		
		var encodingInfo = new EncodingInfo()
		{
			Version = Version.Simple,
			ByteSize = byteSize,
			IsLittleEndian = BitConverter.IsLittleEndian,
			IsFloating =  isFloating,
			IsSigned = isSigned,
		};
		return encodingInfo;
	}
	
	private static void ThrowOnInvalidEncodingInfo<T>(EncodingInfo info)
		where T: struct, INumber<T>
	{
		var typeOfT = typeof(T);
		if (!_byteSizes.TryGetValue(typeOfT, out var byteSize))
		{
			throw new ArgumentException($"Unsupported type {typeOfT}");
		}
		
		var isFloating = _floatingTypes.Contains(typeOfT);
		var isSigned = _signedTypes.Contains(typeOfT);
		
		if(info.ByteSize != byteSize)
		{
			throw new ArgumentException($"Expected {nameof(info.ByteSize)} to be {byteSize}, but was {info.ByteSize}");
		}
		
		if(info.IsFloating != isFloating)
		{
			throw new ArgumentException($"Expected {nameof(info.IsFloating)} to be {isFloating}, but was {info.IsFloating}");
		}
		
		if(info.IsSigned != isSigned)
		{
			throw new ArgumentException($"Expected {nameof(info.IsSigned)} to be {isSigned}, but was {info.IsSigned}");
		}
	}

	public static byte[] SerializeInt32<TBin, TItem>(TBin bin, IList<TItem> items)
		where TBin : IWithDimensions<int>
		where TItem: IWithDimensions<int>, IWithCoordinates<int>
	{
		return SerializeInternal<TBin, TItem, int>(bin, items, x=> x.WriteInt32);
	}
	
	public static byte[] SerializeUInt16<TBin, TItem>(TBin bin, IList<TItem> items)
		where TBin : IWithDimensions<ushort>
		where TItem: IWithDimensions<ushort>, IWithCoordinates<ushort>
	{
		return SerializeInternal<TBin, TItem, ushort>(bin, items, x=> x.WriteUInt16);
	}
	
	private static byte[] SerializeInternal<TBin, TItem, T>(
		TBin bin, 
		IList<TItem> items,
		Func<PackingVisualizationProtocolWriter, Action<T>> writeActionSelector
	)
		where T : struct, INumber<T>
		where TBin : IWithDimensions<T>
		where TItem: IWithDimensions<T>, IWithCoordinates<T>
	{
		if(items.Count > ushort.MaxValue)
		{
			throw new ArgumentOutOfRangeException($"{nameof(items)} cannot be more than {ushort.MaxValue}");
		}

		var header = new Header
		{
			EncodingInfo = CreateEncodingInfo<T>(),
			NumberOfItems = (ushort)items.Count
		};
		using var memoryStream = new MemoryStream();
		using var protocolWriter = new PackingVisualizationProtocolWriter(memoryStream, header.EncodingInfo.IsLittleEndian);
		var encodingInfoByte = EncodingInfo.ToByte(header.EncodingInfo);
		protocolWriter.WriteByte(encodingInfoByte);
		protocolWriter.WriteUInt16(header.NumberOfItems);
		var writeAction = writeActionSelector(protocolWriter);
		writeAction(bin.Length);
		writeAction(bin.Width);
		writeAction(bin.Height);
		foreach (var item in items)
		{
			writeAction(item.Length);
			writeAction(item.Width);
			writeAction(item.Height);
			writeAction(item.X);
			writeAction(item.Y);
			writeAction(item.Z);
		}
	
		using var compressedStream = new MemoryStream();
		using (var compressionStream = new GZipStream(compressedStream, CompressionLevel.Optimal, leaveOpen: true))
		{
			memoryStream.Position = 0; // Reset memory stream to the beginning
			memoryStream.CopyTo(compressionStream);
		}

		return compressedStream.ToArray();
	}
	
	
	public static (TBin, IList<TItem>) DeserializeInt32<TBin, TItem>(byte[] data)
		where TBin : IWithDimensions<int>, new()
		where TItem : IWithDimensions<int>, IWithCoordinates<int>, new()
	{
		return DeserializeInternal<TBin, TItem, int>(data, x=> x.ReadInt32);
	}
	
	public static (TBin, IList<TItem>) DeserializeUInt16<TBin, TItem>(byte[] data)
		where TBin : IWithDimensions<ushort>, new()
		where TItem : IWithDimensions<ushort>, IWithCoordinates<ushort>, new()
	{
		return DeserializeInternal<TBin, TItem, ushort>(data, x=> x.ReadUInt16);
	}
	
	private static (TBin, IList<TItem>) DeserializeInternal<TBin, TItem, T>(
		byte[] data,
		Func<PackingVisualizationProtocolReader, Func<T>> readActionSelector
	)
		where T : struct, INumber<T>
		where TBin : IWithDimensions<T>, new()
		where TItem : IWithDimensions<T>, IWithCoordinates<T>, new()
	{
		using var memoryStream = new MemoryStream(data);
		using var inputStream = new GZipStream(memoryStream, CompressionMode.Decompress);

		int firstByte = inputStream.ReadByte();
		if (firstByte == -1)
		{
			throw new InvalidOperationException("The decompressed data is empty.");
		}

		var readEncodingInfo = EncodingInfo.FromByte((byte)firstByte);

		// Validate the encoding info
		ThrowOnInvalidEncodingInfo<T>(readEncodingInfo);
		
		using var protocolReader = new PackingVisualizationProtocolReader(inputStream, readEncodingInfo.IsLittleEndian);
		var numberOfItems = protocolReader.ReadUInt16();

		var readAction = readActionSelector(protocolReader);
		
		var bin = new TBin()
		{
			Length = readAction(),
			Width = readAction(),
			Height = readAction()
		};
		var items = new List<TItem>();
		for (int i = 0; i < numberOfItems; i++)
		{
			var item = new TItem()
			{
				Length = readAction(),
				Width = readAction(),
				Height = readAction(),
				X = readAction(),
				Y = readAction(),
				Z = readAction()
			};
			items.Add(item);
		}
		return (bin, items);
	}
	
}
