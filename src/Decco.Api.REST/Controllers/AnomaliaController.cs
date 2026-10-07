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

    // Data is optional: `{ "data": {} }` or `{ "data": null }` return the first page of 50.
    [HttpPost("List")]
    public async Task<PagedResponse<AnomaliaDto>> List([FromBody] RequestBase<PageRequest?> request)
    {
        var page = request.Data ?? new PageRequest();
        return await _service.List(page.PageIndex, page.PageSize);
    }

    [HttpPost("Get")]
    public async Task<SingleResponse<AnomaliaDto>> Get([FromBody] RequestBase<int> request)
    {
        return await _service.Get(request.Data);
    }

    [HttpPost("Insert")]
    public async Task<SingleResponse<int>> Insert([FromBody] RequestBase<AnomaliaDto> request)
    {
        return await _service.Insert(request.Data);
    }

    [HttpPost("Update")]
    public async Task<SingleResponse<bool>> Update([FromBody] RequestBase<AnomaliaDto> request)
    {
        return await _service.Update(request.Data);
    }

    [HttpPost("Delete")]
    public async Task<SingleResponse<bool>> Delete([FromBody] RequestBase<int> request)
    {
        return await _service.Delete(request.Data);
    }
}
