﻿using Binacle.Net.Lib.Abstractions.Models;

namespace Binacle.Net.Lib.Fitting.Algorithms;

internal sealed partial class FirstFitDecreasing_v1
{
	private sealed class Bin : VolumetricItem, IWithID
	{
		public Bin(string id, IWithReadOnlyDimensions item) :
			base(item)
		{
			this.ID = id;
		}

		public Bin(string id, int length, int width, int height) :
			base(length, width, height)
		{
			this.ID = id;
		}

		public string ID { get; set; }
	}
}
