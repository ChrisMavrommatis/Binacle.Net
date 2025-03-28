﻿namespace Binacle.Net.Api.v3.Models.Errors;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public class FieldValidationError : IApiError
{
	public required string Field { get; set; }
	public required string Error { get; set; }
}
