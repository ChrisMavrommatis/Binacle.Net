﻿using Binacle.Net.Api.v2.Models;

namespace Binacle.Net.Api.v2.Requests;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public class CustomFitRequest : IWithFitRequestParameters
{
	public FitRequestParameters? Parameters { get; set; }
	public List<Bin>? Bins { get; set; }
	public List<Box>? Items { get; set; }
}
