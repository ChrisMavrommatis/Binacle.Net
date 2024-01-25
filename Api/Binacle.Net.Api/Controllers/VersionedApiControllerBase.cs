using Microsoft.AspNetCore.Mvc;

namespace Binacle.Net.Api.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public abstract class VersionedApiControllerBase : ApiControllerBase
{
    
}
