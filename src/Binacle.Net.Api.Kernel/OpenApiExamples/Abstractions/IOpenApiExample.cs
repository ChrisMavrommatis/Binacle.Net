namespace OpenApiExamples.Abstractions;

public interface IOpenApiExample<T> : IOpenApiExample
{
	string Key { get;  }
	string Summary { get;  }
	string Description { get;  }
	T Value { get;  }
	
}

public interface IOpenApiExample
{
	string Key { get; }
	string Summary { get; }
	string Description { get; }
	object Value { get; }
}
