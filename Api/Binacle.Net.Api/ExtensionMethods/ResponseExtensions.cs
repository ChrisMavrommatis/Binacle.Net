using Binacle.Net.Api.Models.Responses;
using Binacle.Net.Api.Models.Responses.Errors;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Binacle.Net.Api.ExtensionMethods;

public static class ResponseExtensions
{
    public static ErrorResponse AddModelStateErrors(this ErrorResponse response, ModelStateDictionary modelState)
    {
        foreach (var (key, value) in modelState)
        {
            if (value?.ValidationState == ModelValidationState.Invalid)
            {
                foreach (var error in value.Errors)
                {
                    response.Errors.Add(new FieldValidationError() { Field = key, Error = error.ErrorMessage });
                }
            }
        }

        return response;
    }

    public static ErrorResponse AddFieldValidationError(this ErrorResponse response, string field, string message)
    {
        response.Errors.Add(new FieldValidationError { Field = field, Error = message });

        return response;
    }

    public static ErrorResponse AddParameterError(this ErrorResponse response, string parameter, string message)
    {
        response.Errors.Add(new ParameterError { Parameter = parameter, Mesasage = message });

        return response;
    }
}
