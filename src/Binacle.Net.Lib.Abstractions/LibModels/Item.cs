﻿using Binacle.Net.Lib.Abstractions.Models;

namespace Binacle.Net.Lib.Models;

public class Item : IWithID, IWithReadOnlyDimensions, IWithQuantity
{
	public Item(string id, IWithReadOnlyDimensions item, int quantity) :
		this(id, item.Length, item.Width, item.Height, quantity)
	{

	}
	public Item(string id, int length, int width, int height, int quantity)
	{
		this.ID = id;
		this.Length = length;
		this.Width = width;
		this.Height = height;
		this.Quantity = quantity;
	}

	public string ID { get; set; }
	public int Length { get; }
	public int Width { get; }
	public int Height { get; }
	public int Quantity { get; set; }
}
