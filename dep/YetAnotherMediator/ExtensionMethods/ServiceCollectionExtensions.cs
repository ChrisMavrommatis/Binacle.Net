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
		services.AddTransient<IRequestHandler<TRequest, TResponse>, THandler>();
		return services;
	}
	
	public static IServiceCollection AddRequestHandler<THandler>(
		this IServiceCollection services)
		where THandler : class, IRequestHandler
	{
		var handlerInterface = typeof(THandler)
			.GetInterfaces()
			.FirstOrDefault(i =>
				i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>));

		if (handlerInterface == null)
			throw new InvalidOperationException($"{typeof(THandler).Name} does not implement IRequestHandler<,>");

		services.AddTransient(handlerInterface, typeof(THandler));
		return services;
	}

	// Add Query Handler
	public static IServiceCollection AddQueryHandler<TQuery, TResult, THandler>(
		this IServiceCollection services)
		where TQuery : IQuery<TResult>
		where THandler : class, IQueryHandler<TQuery, TResult>
	{
		services.AddTransient<IQueryHandler<TQuery, TResult>, THandler>();
		return services;
	}
	
	public static IServiceCollection AddQueryHandler<THandler>(
		this IServiceCollection services)
		where THandler : class, IQueryHandler
	{
		var handlerInterface = typeof(THandler)
			.GetInterfaces()
			.FirstOrDefault(i =>
				i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>));

		if (handlerInterface == null)
			throw new InvalidOperationException($"{typeof(THandler).Name} does not implement IQueryHandler<,>");

		services.AddTransient(handlerInterface, typeof(THandler));
		return services;
	}
	
	// Add Command Handler
	public static IServiceCollection AddCommandHandler<TCommand, TResult, THandler>(
		this IServiceCollection services)
		where TCommand : ICommand<TResult>
		where THandler : class, ICommandHandler<TCommand, TResult>
	{
		services.AddTransient<ICommandHandler<TCommand, TResult>, THandler>();
		return services;
	}
	
	public static IServiceCollection AddCommandHandler<THandler>(this IServiceCollection services)
		where THandler : class, ICommandHandler
	{
		var handlerInterface = typeof(THandler)
			.GetInterfaces()
			.FirstOrDefault(i =>
				i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>));

		if (handlerInterface == null)
			throw new InvalidOperationException($"{typeof(THandler).Name} does not implement ICommandHandler<,>");

		services.AddTransient(handlerInterface, typeof(THandler));
		return services;
	}
}
