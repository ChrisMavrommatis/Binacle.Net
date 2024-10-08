using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json.Serialization;

namespace Binacle.Net.Api.v1.Responses;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public class ErrorResponse : v1.Models.ResponseBase
{
	public ErrorResponse()
	{
		Result = v1.Models.ResultType.Error;
		Errors = new List<v1.Models.Errors.IApiError>();
	}

	public static ErrorResponse Create(string? message = null)
	{
		return new ErrorResponse { Message = message };
	}

	[JsonPropertyOrder(99)]
	public List<v1.Models.Errors.IApiError> Errors { get; set; }

	public ErrorResponse AddModelStateErrors(ModelStateDictionary modelState)
	{
		foreach (var (key, value) in modelState)
		{
			if (value?.ValidationState == ModelValidationState.Invalid)
			{
				foreach (var error in value.Errors)
				{
					AddFieldValidationError(key, error.ErrorMessage);
				}
			}
		}

		return this;
	}

	public ErrorResponse AddFieldValidationError(string field, string message)
	{
		this.Errors.Add(new v1.Models.Errors.FieldValidationError { Field = field, Error = message });

		return this;
	}

	public ErrorResponse AddParameterError(string parameter, string message)
	{
		this.Errors.Add(new v1.Models.Errors.ParameterError { Parameter = parameter, Message = message });

		return this;
	}

	public ErrorResponse AddExceptionError(Exception ex)
	{
		// if environment is development  then add the error
		if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
		{
			this.Errors.Add(new v1.Models.Errors.ExceptionError() { ExceptionType = ex.GetType().Name, Message = ex.Message, StackTrace = ex.StackTrace });
		}

		return this;
	}
}
