using Binacle.Net.Api.v2.Models;
using ChrisMavrommatis.Endpoints.Requests;

namespace Binacle.Net.Api.v2.Requests;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public class CustomFitRequestWithBody : RequestWithBody<CustomFitRequest>
{
}
public class CustomFitRequest
{
	public FitRequestParameters? Parameters { get; set; }
	public List<Bin> Bins { get; set; }
	public List<Box> Items { get; set; }
}
