using Binacle.Net.Api.Models.Responses.Errors;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json.Serialization;

namespace Binacle.Net.Api.Models.Responses;

public class ErrorResponse : ResponseBase
{
    public ErrorResponse()
    {
        this.Result = ResultType.Error;
        this.Errors = new List<IApiError>();
    }

    [JsonPropertyOrder(99)]
    public List<IApiError> Errors { get; set; }

	public ErrorResponse AddModelStateErrors(ModelStateDictionary modelState)
	{
		foreach (var (key, value) in modelState)
		{
			if (value?.ValidationState == ModelValidationState.Invalid)
			{
				foreach (var error in value.Errors)
				{
					this.AddFieldValidationError(key, error.ErrorMessage);
				}
			}
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
		this.Errors.Add(new ExceptionError() { ExceptionType = ex.GetType().Name, Message = ex.Message, StackTrace = ex.StackTrace });
		return this;

	}
}
