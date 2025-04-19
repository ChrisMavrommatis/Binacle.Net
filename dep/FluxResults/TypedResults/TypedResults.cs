using FluxResults.Abstractions.TypedResults;

namespace FluxResults.TypedResults;

public readonly record struct Success(string? Message) : ISuccessfulTypedResult;
public readonly record struct Created(string? Message) : ISuccessfulTypedResult;
public readonly record struct Deleted(string? Message) : ISuccessfulTypedResult;
public readonly record struct Updated(string? Message) : ISuccessfulTypedResult;

public readonly record struct Conflict(string? Message) : IErrorTypedResult;
public readonly record struct NotFound(string? Message) : IErrorTypedResult;
public readonly record struct ValidationError(string? Message) : IErrorTypedResult;
public readonly record struct Unauthorized(string? Message) : IErrorTypedResult;
public readonly record struct Forbidden(string? Message) : IErrorTypedResult;
public readonly record struct UnexpectedError(string? Message) : IErrorTypedResult;

public static class TypedResult
{
	public static Success Success => default;
	public static Success SuccessWith(string message) => new(message);
	
	public static Created Created => default;
	public static Created CreatedWith(string message) => new(message);
	
	public static Deleted Deleted => default;
	public static Deleted DeletedWith(string message) => new(message);
	
	public static Updated Updated => default;
	public static Updated UpdatedWith(string message) => new(message);
	
	public static Conflict Conflict => default;
	public static Conflict ConflictWith(string message) => new(message);
	
	public static NotFound NotFound => default;
	public static NotFound NotFoundWith(string message) => new(message);
	
	public static ValidationError ValidationError => default;
	public static ValidationError ValidationErrorWith(string message) => new(message);
	
	public static Unauthorized Unauthorized => default;
	public static Unauthorized UnauthorizedWith(string message) => new(message);
	
	public static Forbidden Forbidden => default;
	public static Forbidden ForbiddenWith(string message) => new(message);
	
	public static UnexpectedError UnexpectedError => default;
	public static UnexpectedError UnexpectedErrorWith(string message) => new(message);

}
