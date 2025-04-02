using Binacle.Net.Api.v2.Models.Errors;
using Binacle.Net.Api.v2.Responses;
using FluentValidation.Results;

namespace Binacle.Net.Api.v2.Models;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

internal static class Response
{
	private static ErrorResponse Error(List<IApiError> data, string message)
	{
		return new ErrorResponse
		{
			Result = ResultType.Error,
			Message = message,
			Data = data
		};
	}


	public static ErrorResponse ParameterError(string parameter, string parameterMessage, string message)
	{
		var errors = new List<IApiError>
		{
			new ParameterError
			{
				Parameter = parameter,
				Message = parameterMessage
			}
		};

		return Error(errors, message);
	}

	public static ErrorResponse FieldValidationError(string field, string error, string message)
	{
		var errors = new List<IApiError>
		{
			new FieldValidationError
			{
				Field = field,
				Error = error
			}
		};

		return Error(errors, message);
	}

	public static ErrorResponse ValidationError(ValidationResult validationResult, string message)
	{
		var errors = new List<IApiError>();
		foreach (var error in validationResult.Errors)
		{
			errors.Add(new FieldValidationError { Field = error.PropertyName, Error = error.ErrorMessage });
		}

		return Error(errors, message);
	}


	public static ErrorResponse ExceptionError(Exception ex, string message)
	{
		var errors = new List<IApiError>();
		if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
		{
			errors.Add(new ExceptionError() { ExceptionType = ex.GetType().Name, Message = ex.Message, StackTrace = ex.StackTrace });
		}
		var response = new ErrorResponse
		{
			Result = ResultType.Error,
			Message = message,
			Data = new List<IApiError>()
		};
		return Error(errors, message);
	}
}
