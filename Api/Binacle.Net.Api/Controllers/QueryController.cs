using Asp.Versioning;
using Binacle.Net.Api.Configuration.Models;
using Binacle.Net.Api.Models.Requests;
using Binacle.Net.Api.Services;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace Binacle.Net.Api.Controllers;

[ApiVersion("1.0")]
public partial class QueryController : VersionedApiControllerBase
{
	private readonly IValidator<QueryRequest> queryRequestValidator;
	private readonly IValidator<PresetQueryRequest> presetQueryRequestValidator;
	private readonly IOptions<BinPresetOptions> presetOptions;
	private readonly ILockerService lockerService;
	public QueryController(
		IValidator<QueryRequest> queryRequestValidator,
		IValidator<PresetQueryRequest> presetQueryRequestValidator,
		IOptions<BinPresetOptions> presetOptions,
		ILockerService lockerService
	  )
	{
		this.queryRequestValidator = queryRequestValidator;
		this.presetQueryRequestValidator = presetQueryRequestValidator;
		this.presetOptions = presetOptions;
		this.lockerService = lockerService;
	}

}
