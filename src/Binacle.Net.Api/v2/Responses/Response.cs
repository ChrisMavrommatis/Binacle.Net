using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Binacle.Net.Api.v2.Responses;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

internal static class Response
{
	private static ErrorResponse Error(List<v2.Models.Errors.IApiError> data, string message)
	{
		return new ErrorResponse
		{
			Result = v2.Models.ResultType.Error,
			Message = message,
			Data = data
		};
	}


	public static ErrorResponse ParameterError(string parameter, string parameterMessage, string message)
	{
		var errors = new List<v2.Models.Errors.IApiError>
		{
			new v2.Models.Errors.ParameterError
			{
				Parameter = parameter,
				Message = parameterMessage
			}
		};

		return Error(errors, message);
	}

	public static ErrorResponse FieldValidationError(string field, string error, string message)
	{
		var errors = new List<v2.Models.Errors.IApiError>
		{
			new v2.Models.Errors.FieldValidationError
			{
				Field = field,
				Error = error
			}
		};

		return Error(errors, message);
	}

	public static ErrorResponse ValidationError(ModelStateDictionary modelState, string message)
	{
		var errors = new List<v2.Models.Errors.IApiError>();

		foreach (var (key, value) in modelState)
		{
			if (value?.ValidationState == ModelValidationState.Invalid)
			{
				foreach (var error in value.Errors)
				{
					errors.Add(new v2.Models.Errors.FieldValidationError { Field = key, Error = error.ErrorMessage });
				}
			}
		}

		return Error(errors, message);
	}


	public static ErrorResponse ExceptionError(Exception ex, string message)
	{
		var errors = new List<v2.Models.Errors.IApiError>();
		if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
		{
			errors.Add(new v2.Models.Errors.ExceptionError() { ExceptionType = ex.GetType().Name, Message = ex.Message, StackTrace = ex.StackTrace });
		}
		var response = new ErrorResponse
		{
			Result = v2.Models.ResultType.Error,
			Message = message,
			Data = new List<v2.Models.Errors.IApiError>()
		};
		return Error(errors, message);
	}
}
