namespace Binacle.Net.TestsKernel.Helpers;

public static class DimensionHelper
{
	public static Models.TestItem ParseFromCompactString(string compactString)
	{
		// Items parse from string with pattern q-dxdxd
		var parts = compactString.Split('-');
		if (parts.Length > 2)
		{
			throw new ArgumentException($"Invalid format. Value {compactString} should have format 'Q-DxDxD' or 'DxDxD'.");
		}

		var quantity = 1;
		string dimensionString;
		if (parts.Length == 2)
		{
			if (!int.TryParse(parts[0], out int quantityResult))
			{
				throw new ArgumentException($"Invalid quantity number. Value {compactString} should have format 'Q-DxDxD' or 'DxDxD'.");
			}
			quantity = quantityResult;
			dimensionString = parts[1];
		}
		else
		{
			dimensionString = parts[0];

		}
		var dimensions = dimensionString.Split('x');

		if (dimensions.Length != 3)
		{
			throw new ArgumentException($"Invalid dimension format. Value {compactString} should have format 'Q-DxDxD' or 'DxDxD'.");
		}

		if (!int.TryParse(dimensions[0], out int length)
			|| !int.TryParse(dimensions[1], out int width)
			|| !int.TryParse(dimensions[2], out int height)
			)
		{
			throw new ArgumentException($"Invalid dimension number. Value {compactString} should have format 'Q-DxDxD' or 'DxDxD'.");
		}

		return new Models.TestItem
		{
			ID = compactString,
			Quantity = quantity,
			Length = length,
			Width = width,
			Height = height
		};
	}

}
