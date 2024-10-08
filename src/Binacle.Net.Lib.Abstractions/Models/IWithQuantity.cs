﻿using System.Numerics;

namespace Binacle.Net.Lib.Abstractions.Models;

public interface IWithQuantity : IWithQuantity<int>
{
}

public interface IWithQuantity<T>
	where T : INumber<T>
{
	T Quantity { get; set; }
}
