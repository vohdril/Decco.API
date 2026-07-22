using Decco.Api.Services;
using Decco.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Decco.Api.REST.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnomaliaController : ControllerBase
{
    private readonly IAnomaliaService _service;

    public AnomaliaController(IAnomaliaService service)
    {
        _service = service;
    }

    [HttpPost("ListAnomalias")]
    public async Task<PagedResponse<AnomaliaDto>> List([FromBody] RequestBase<object> request)
    {
        return await _service.ListAnomalias();
    }

    [HttpPost("GetAnomalia")]
    public async Task<SingleResponse<AnomaliaDto>> Get([FromBody] RequestBase<int> request)
    {
        return await _service.GetAnomalia(request.Data);
    }

    [HttpPost("InsertAnomalia")]
    public async Task<SingleResponse<int>> Insert([FromBody] RequestBase<AnomaliaDto> request)
    {
        return await _service.InsertAnomalia(request.Data);
    }

    [HttpPost("UpdateAnomalia")]
    public async Task<SingleResponse<bool>> Update([FromBody] RequestBase<AnomaliaDto> request)
    {
        return await _service.UpdateAnomalia(request.Data);
    }

    [HttpPost("DeleteAnomalia")]
    public async Task<SingleResponse<bool>> Delete([FromBody] RequestBase<int> request)
    {
        return await _service.DeleteAnomalia(request.Data);
    }
}
