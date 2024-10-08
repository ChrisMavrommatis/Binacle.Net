using System.Net;
using System.Net.Sockets;

namespace Binacle.Net.Api.DiagnosticsModule.Models;

public class IPAddressRange
{
	readonly AddressFamily addressFamily;
	readonly byte[] lowerBytes;
	readonly byte[] upperBytes;

	public IPAddressRange(IPAddress lowerInclusive, IPAddress upperInclusive)
	{
		// Assert that lower.AddressFamily == upper.AddressFamily
		if(lowerInclusive.AddressFamily != upperInclusive.AddressFamily)
		{
			throw new ArgumentException("The lower and upper addresses have different address families");
		}

		this.addressFamily = lowerInclusive.AddressFamily;
		this.lowerBytes = lowerInclusive.GetAddressBytes();
		this.upperBytes = upperInclusive.GetAddressBytes();
	}

	internal static IPAddressRange ParseRange(string ipRange)
	{
		// Format: xxx.xxx.xxx.xxx-xxx.xxx.xxx.xxx
		if (ipRange.Contains("-"))
		{
			string[] ips = ipRange.Split('-');
			return new IPAddressRange(IPAddress.Parse(ips[0]), IPAddress.Parse(ips[1]));
		}

		// Format: xxx.xxx.xxx.xxx/xx
		if (ipRange.Contains("/"))
		{
			string[] parts = ipRange.Split('/');
			byte[] addressBytes = IPAddress.Parse(parts[0]).GetAddressBytes();
			byte[] maskBytes = IPAddress.Parse(parts[1]).GetAddressBytes();
			byte[] lowerBytes = new byte[addressBytes.Length];
			byte[] upperBytes = new byte[addressBytes.Length];
			for (int i = 0; i < addressBytes.Length; i++)
			{
				lowerBytes[i] = (byte)(addressBytes[i] & maskBytes[i]);
				upperBytes[i] = (byte)(addressBytes[i] | ~maskBytes[i]);
			}
			return new IPAddressRange(new IPAddress(lowerBytes), new IPAddress(upperBytes));
		}

		// Format: xxx.xxx.xxx.xxx
		return new IPAddressRange(IPAddress.Parse(ipRange), IPAddress.Parse(ipRange));
	}

	public bool IsInRange(IPAddress address)
	{
		if (address.AddressFamily != addressFamily)
		{
			return false;
		}

		byte[] addressBytes = address.GetAddressBytes();

		bool lowerBoundary = true, upperBoundary = true;

		for (int i = 0; i < this.lowerBytes.Length &&
			(lowerBoundary || upperBoundary); i++)
		{
			if ((lowerBoundary && addressBytes[i] < lowerBytes[i]) ||
				(upperBoundary && addressBytes[i] > upperBytes[i]))
			{
				return false;
			}

			lowerBoundary &= (addressBytes[i] == lowerBytes[i]);
			upperBoundary &= (addressBytes[i] == upperBytes[i]);
		}

		return true;
	}
}
