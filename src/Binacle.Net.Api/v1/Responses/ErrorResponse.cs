using System.Text;
using System.Text.Json.Serialization;
using Binacle.Net.Api.v1.Models;
using Binacle.Net.Api.v1.Models.Errors;
using FluentValidation.Results;

namespace Binacle.Net.Api.v1.Responses;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public class ErrorResponse : ResponseBase
{
	public ErrorResponse()
	{
		Result = ResultType.Error;
		Errors = new List<IApiError>();
	}

	public static ErrorResponse Create(string message)
	{
		return new ErrorResponse { Message = message };
	}

	[JsonPropertyOrder(99)]
	public List<IApiError> Errors { get; set; }

	public ErrorResponse AddValidationResult(ValidationResult validationResult)
	{
		foreach (var error in validationResult.Errors)
		{
			AddFieldValidationError(error.PropertyName, error.ErrorMessage);
		}

		return this;
	}

	public ErrorResponse AddFieldValidationError(string field, string message)
	{
		this.Errors.Add(new FieldValidationError { Field = field, Error = message });

		return this;
	}

	public ErrorResponse AddParameterError(string parameter, string message)
	{
		this.Errors.Add(new ParameterError { Parameter = parameter, Message = message });

		return this;
	}

	public ErrorResponse AddExceptionError(Exception ex)
	{
		// if environment is development  then add the error
		if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
		{
			this.Errors.Add(new ExceptionError() { ExceptionType = ex.GetType().Name, Message = ex.Message, StackTrace = ex.StackTrace });
		}

		return this;
	}
}
