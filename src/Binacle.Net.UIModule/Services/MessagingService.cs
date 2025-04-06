using System.Collections.Concurrent;

namespace Binacle.Net.UIModule.Services;


internal class MessagingService
{
	private readonly ConcurrentDictionary<string, List<Func<Task>>> _notifications  = new();
	private readonly ConcurrentDictionary<string, List<Delegate>> _broadcasts  = new();
	private readonly ConcurrentDictionary<string, Delegate> _requests = new();


	public void On(string notification, Func<Task> handler)
	{
		if (!_notifications.ContainsKey(notification))
		{
			_notifications.TryAdd(notification, new List<Func<Task>>());
		}

		_notifications[notification].Add(handler);
	}
	
	public void Off(string notification)
	{
		_notifications.TryRemove(notification, out _);
	}

	public async Task TriggerAsync(string notification)
	{
		if (_notifications.TryGetValue(notification, out var @event))
		{
			foreach (var handler in @event)
			{
				await handler();
			}
		}
	}

	public void On<TData>(string eventName, Func<TData, Task> handler)
	{
		if (!_broadcasts.ContainsKey(eventName))
		{
			_broadcasts.TryAdd(eventName, new List<Delegate>());
		}

		_broadcasts[eventName].Add(handler);
	}
	
	public void Off<TData>(string eventName)
	{
		_broadcasts.TryRemove(eventName, out _);
	}

	public async Task TriggerAsync<TData>(string eventName, TData data)
	{
		if (_broadcasts.TryGetValue(eventName, out var @event))
		{
			foreach (var handler in @event)
			{
				await ((Func<TData, Task>)handler)(data);
			}
		}
	}

	public void On<TRequest, TResponse>(string eventName, Func<TRequest, Task<TResponse>> handler)
	{
		if (!_requests.TryAdd(eventName, handler))
		{
			throw new InvalidOperationException($"Only one handler can be registered for the event '{eventName}'.");
		}
	}
	
	public void Off<TRequest, TResponse>(string eventName)
	{
		_requests.TryRemove(eventName, out _);
	}
	
	public async Task<TResponse> TriggerAsync<TRequest, TResponse>(string eventName, TRequest request)
	{
		if (_requests.TryGetValue(eventName, out var handler))
		{
			return await ((Func<TRequest, Task<TResponse>>)handler)(request);
		}

		throw new InvalidOperationException($"No handler registered for the event '{eventName}'.");
	}
}
