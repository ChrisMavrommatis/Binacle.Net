﻿using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Binacle.Net.Api.v3.Models.Errors;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(FieldValidationError), nameof(FieldValidationError))]
[JsonDerivedType(typeof(ParameterError), nameof(ParameterError))]
[JsonDerivedType(typeof(ExceptionError), nameof(ExceptionError))]
// [XmlInclude(typeof(FieldValidationError))]
// [XmlInclude(typeof(ParameterError))]
// [XmlInclude(typeof(ExceptionError))]
public interface IApiError
{
	public static string TypeName { get; set; }
}
