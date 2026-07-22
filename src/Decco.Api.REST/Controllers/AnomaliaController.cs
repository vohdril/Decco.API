using Decco.Api.Operations;
using Decco.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Decco.Api.REST.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnomaliaController : ControllerBase
{
    private readonly IAnomaliaManager _manager;

    public AnomaliaController(IAnomaliaManager manager)
    {
        _manager = manager;
    }

    [HttpPost("ListAnomalias")]
    public async Task<PagedResponse<AnomaliaDto>> List([FromBody] RequestBase<object> request)
    {
        return await _manager.ListAnomalias();
    }

    [HttpPost("GetAnomalia")]
    public async Task<SingleResponse<AnomaliaDto>> Get([FromBody] RequestBase<int> request)
    {
        return await _manager.GetAnomalia(request.Data);
    }

    [HttpPost("InsertAnomalia")]
    public async Task<SingleResponse<int>> Insert([FromBody] RequestBase<AnomaliaDto> request)
    {
        return await _manager.InsertAnomalia(request.Data);
    }

    [HttpPost("UpdateAnomalia")]
    public async Task<SingleResponse<bool>> Update([FromBody] RequestBase<AnomaliaDto> request)
    {
        return await _manager.UpdateAnomalia(request.Data);
    }

    [HttpPost("DeleteAnomalia")]
    public async Task<SingleResponse<bool>> Delete([FromBody] RequestBase<int> request)
    {
        return await _manager.DeleteAnomalia(request.Data);
    }
}
