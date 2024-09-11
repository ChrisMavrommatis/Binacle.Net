using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Binacle.Net.Api.v2.Models;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

internal static class Response
{
	private static v2.Responses.ErrorResponse Error(List<Errors.IApiError> data, string message)
	{
		return new v2.Responses.ErrorResponse
		{
			Result = ResultType.Error,
			Message = message,
			Data = data
		};
	}


	public static v2.Responses.ErrorResponse ParameterError(string parameter, string parameterMessage, string message)
	{
		var errors = new List<Errors.IApiError>
		{
			new Errors.ParameterError
			{
				Parameter = parameter,
				Message = parameterMessage
			}
		};

		return Error(errors, message);
	}

	public static v2.Responses.ErrorResponse FieldValidationError(string field, string error, string message)
	{
		var errors = new List<Errors.IApiError>
		{
			new Errors.FieldValidationError
			{
				Field = field,
				Error = error
			}
		};

		return Error(errors, message);
	}

	public static v2.Responses.ErrorResponse ValidationError(ModelStateDictionary modelState, string message)
	{
		var errors = new List<Errors.IApiError>();

		foreach (var (key, value) in modelState)
		{
			if (value?.ValidationState == ModelValidationState.Invalid)
			{
				foreach (var error in value.Errors)
				{
					errors.Add(new Errors.FieldValidationError { Field = key, Error = error.ErrorMessage });
				}
			}
		}

		return Error(errors, message);
	}


	public static v2.Responses.ErrorResponse ExceptionError(Exception ex, string message)
	{
		var errors = new List<Errors.IApiError>();
		if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
		{
			errors.Add(new Errors.ExceptionError() { ExceptionType = ex.GetType().Name, Message = ex.Message, StackTrace = ex.StackTrace });
		}
		var response = new v2.Responses.ErrorResponse
		{
			Result = ResultType.Error,
			Message = message,
			Data = new List<Errors.IApiError>()
		};
		return Error(errors, message);
	}
}
