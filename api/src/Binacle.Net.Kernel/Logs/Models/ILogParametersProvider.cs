namespace Binacle.Net.Kernel.Logs.Models;

// Projects a request's parameters into the log — a free-form list of strings, deliberately loose because the
// values are just extra info and may change from one API version to the next. The API's parameter types implement
// this because the background converter can't see their (v3 / v4) enums. Called by the converter, in the
// background — never on the request thread.
public interface ILogParametersProvider
{
	IReadOnlyList<string> ToLogParameters();
}
