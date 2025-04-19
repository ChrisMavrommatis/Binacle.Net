using Microsoft.Extensions.DependencyInjection;

namespace YetAnotherMediator;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddMediator(this IServiceCollection services)
	{
		services.AddSingleton<IMediator, Mediator>();
		return services;
	}
	
	// Add Request Handler
	public static IServiceCollection AddRequestHandler<TRequest, TResponse, THandler>(
		this IServiceCollection services)
		where TRequest : IRequest<TResponse>
		where THandler : class, IRequestHandler<TRequest, TResponse>
	{
		services.AddScoped<IRequestHandler<TRequest, TResponse>, THandler>();
		return services;
	}

	// Add Query Handler
	public static IServiceCollection AddQueryHandler<TQuery, TResult, THandler>(
		this IServiceCollection services)
		where TQuery : IQuery<TResult>
		where THandler : class, IQueryHandler<TQuery, TResult>
	{
		services.AddScoped<IQueryHandler<TQuery, TResult>, THandler>();
		return services;
	}

	// Add Command Handler
	public static IServiceCollection AddCommandHandler<TCommand, TResult, THandler>(
		this IServiceCollection services)
		where TCommand : ICommand<TResult>
		where THandler : class, ICommandHandler<TCommand, TResult>
	{
		services.AddScoped<ICommandHandler<TCommand, TResult>, THandler>();
		return services;
	}
	
	
	
}
