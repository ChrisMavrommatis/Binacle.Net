namespace Binacle.TestsKernel.Helpers;

public static class DimensionsHelper
{
	public static Models.DimensionsAndQuantity ParseFromCompactString(string compactString)
	{
		// Items parse from string with pattern dxdxd-q
		var parts = compactString.Split('-');
		if (parts.Length > 2)
		{
			throw new ArgumentException($"Invalid format. Value {compactString} should have format 'LxWxH-Q' or 'LxWxH'.");
		}

		var quantity = 1;
		string dimensionString;
		if (parts.Length == 2)
		{
			if (!int.TryParse(parts[1], out int quantityResult))
			{
				throw new ArgumentException($"Invalid quantity number. Value {compactString} should have format 'LxWxH-Q' or 'LxWxH'.");
			}
			quantity = quantityResult;
			dimensionString = parts[0];
		}
		else
		{
			dimensionString = parts[0];

		}
		var dimensions = dimensionString.Split('x');

		if (dimensions.Length != 3)
		{
			throw new ArgumentException($"Invalid dimension format. Value {compactString} should have format 'LxWxH-Q' or 'LxWxH'.");
		}

		if (!int.TryParse(dimensions[0], out int length)
		    || !int.TryParse(dimensions[1], out int width)
		    || !int.TryParse(dimensions[2], out int height)
		   )
		{
			throw new ArgumentException($"Invalid dimension number. Value {compactString} should have format 'LxWxH-Q' or 'LxWxH'.");
		}

		return new Models.DimensionsAndQuantity(length, width, height, quantity);
	}

}
