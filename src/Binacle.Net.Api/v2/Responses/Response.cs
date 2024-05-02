using Binacle.Net.Api.Models;
using Binacle.Net.Api.v2.Models;
using Binacle.Net.Api.v2.Models.Errors;
using Binacle.Net.Lib.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json.Serialization;

namespace Binacle.Net.Api.v2.Responses;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public class Response<TModel>
{
	[JsonPropertyOrder(0)]
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public ResultType Result { get; set; }

	public TModel Data { get; set; }

	public string Message { get; set; }

	public static Response<TModel> Success(TModel model, string? message = null)
	{
		return new Response<TModel>
		{
			Result = ResultType.Success,
			Message = message,
			Data = model
		};
	}
}

internal static class Response
{
	public static Response<Bin?> FromResult(BinFittingOperationResult operationResult)
	{
		if (operationResult.Status == BinFitResultStatus.Success)
		{
			var bin = new Bin
			{
				ID = operationResult.FoundBin.ID,
				Height = operationResult.FoundBin.Height,
				Length = operationResult.FoundBin.Length,
				Width = operationResult.FoundBin.Width
			};
			return Success(bin);
		}

		return new Response<Bin?>
		{
			Result = ResultType.Failure,
			Message = $"Failed to find bin. Reason: {operationResult.Reason.ToString()}",
		};
	}

	public static Response<TModel> Success<TModel>(TModel model, string? message = null)
	{
		return new Response<TModel>
		{
			Result = ResultType.Success,
			Message = message,
			Data = model
		};
	}

	private static Response<TModel> Error<TModel>(TModel model, string message)
	{
		return new Response<TModel>
		{
			Result = ResultType.Error,
			Message = message,
			Data = model
		};
	}


	public static Response<List<IApiError>> ParameterError(string parameter, string parameterMessage, string message)
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

	public static Response<List<IApiError>> FieldValidationError(string field, string error, string message)
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

	public static Response<List<IApiError>> ValidationError(ModelStateDictionary modelState, string message)
	{
		var errors = new List<IApiError>();

		foreach (var (key, value) in modelState)
		{
			if (value?.ValidationState == ModelValidationState.Invalid)
			{
				foreach (var error in value.Errors)
				{
					errors.Add(new FieldValidationError { Field = key, Error = error.ErrorMessage });
				}
			}
		}

		return Error(errors, message);
	}


	public static Response<List<IApiError>> ExceptionError(Exception ex, string message)
	{
		var errors = new List<IApiError>();
		if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
		{
			errors.Add(new ExceptionError() { ExceptionType = ex.GetType().Name, Message = ex.Message, StackTrace = ex.StackTrace });
		}
		var response = new Response<List<IApiError>>
		{
			Result = ResultType.Error,
			Message = message,
			Data = new List<IApiError>()
		};
		return Error(errors, message);
	}
}
