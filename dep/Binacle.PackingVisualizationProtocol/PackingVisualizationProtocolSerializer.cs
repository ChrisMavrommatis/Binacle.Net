using System.Numerics;
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
	
	internal static EncodingInfo CreateEncodingInfo<T>()
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

	internal static void ThrowOnInvalidEncodingInfo<T>(EncodingInfo info)
		where T: struct, INumber<T>
	{
		var typeOfT = typeof(T);
		var byteSize = _byteSizes[typeOfT];
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
		
	public static byte[] Serialize<TBin, TItem>(TBin bin, IList<TItem> items)
		where TBin : IWithDimensions<ushort>
		where TItem: IWithDimensions<ushort>, IWithCoordinates<ushort>
	{
		if(items.Count > ushort.MaxValue)
		{
			throw new ArgumentOutOfRangeException($"{nameof(items)} cannot be more than {ushort.MaxValue}");
		}

		var header = new Header
		{
			EncodingInfo = CreateEncodingInfo<ushort>(),
			NumberOfItems = (ushort)items.Count
		};

		return SerializeInternal<TBin, TItem, ushort>(header, bin, items, x=> x.WriteUInt16);
	}
	
	internal static byte[] SerializeInternal<TBin, TItem, T>(
		Header header, 
		TBin bin, 
		IList<TItem> items,
		Func<PackingVisualizationProtocolWriter, Action<T>> writeActionSelector
	)
		where T : struct, INumber<T>
		where TBin : IWithDimensions<T>
		where TItem: IWithDimensions<T>, IWithCoordinates<T>
	{
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
		return memoryStream.ToArray();
	}
	
	public static (TBin, IList<TItem>) Deserialize<TBin, TItem>(byte[] data)
		where TBin : IWithDimensions<ushort>, new()
		where TItem : IWithDimensions<ushort>, IWithCoordinates<ushort>, new()
	{
		var readEncodingInfo = EncodingInfo.FromByte(data[0]);
		
		ThrowOnInvalidEncodingInfo<ushort>(readEncodingInfo);
		
		return DeserializeInternal<TBin, TItem, ushort>(data, readEncodingInfo, x=> x.ReadUInt16);
	}

	internal static (TBin, IList<TItem>) DeserializeInternal<TBin, TItem, T>(
		byte[] data,
		EncodingInfo encodingInfo,
		Func<PackingVisualizationProtocolReader, Func<T>> readActionSelector
	)
		where T : struct, INumber<T>
		where TBin : IWithDimensions<T>, new()
		where TItem : IWithDimensions<T>, IWithCoordinates<T>, new()
	{
		using var memoryStream = new MemoryStream(data);
		using var protocolReader = new PackingVisualizationProtocolReader(memoryStream, encodingInfo.IsLittleEndian);
		var _ = EncodingInfo.FromByte(protocolReader.ReadByte());
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
