using Binacle.Net.Api.UIModule.ApiModels.Requests;

namespace Binacle.Net.Api.UIModule.ApiModels.Responses;

internal class ErrorResponse
{
	public ResponseResultType Result { get; set; }

	List<Error> Data { get; set; }

}
