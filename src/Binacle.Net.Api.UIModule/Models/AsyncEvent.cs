namespace Binacle.Net.Api.UIModule.Models;

internal class AsyncEvent<T>
{
	private Func<T, Task> _handlers;

	// Async method to invoke all the handlers if not null
	public async Task InvokeAsync(T arg)
	{
		if (_handlers is not null)
		{
			foreach (var handler in _handlers.GetInvocationList().Cast<Func<T, Task>>())
			{
				await handler(arg);
			}
		}
	}

	public static implicit operator AsyncEvent<T>(Func<T, Task> handler)
	{
		return new AsyncEvent<T> { _handlers = handler };
	}

	// +=
	public static AsyncEvent<T> operator +(AsyncEvent<T> asyncEvent, Func<T, Task> handler)
	{
		if(asyncEvent is null)
		{
			asyncEvent = new AsyncEvent<T>();
		}

		if(asyncEvent._handlers is null)
		{
			 asyncEvent._handlers = handler;
		}
		else
		{
			asyncEvent._handlers += handler;
		}
		return asyncEvent;
	}

	// -=
	public static AsyncEvent<T> operator -(AsyncEvent<T> asyncEvent, Func<T, Task> handler)
	{
		if (asyncEvent is null)
		{
			asyncEvent = new AsyncEvent<T>();
		}

		if (asyncEvent._handlers is not null)
		{
			asyncEvent._handlers -= handler;
		}

		return asyncEvent;
	}
}
