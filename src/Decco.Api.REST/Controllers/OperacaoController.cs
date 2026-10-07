using Decco.Api.Services;
using Decco.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Decco.Api.REST.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OperacaoController : ControllerBase
{
    private readonly IOperacaoService _service;

    public OperacaoController(IOperacaoService service)
    {
        _service = service;
    }

    // The List body is the FILTER (OperacaoFilterDto) — that is why the API uses POST:
    // the contract describes the slice (facility, type, status, clearance, page).
    [HttpPost("List")]
    public async Task<PagedResponse<OperacaoDto>> List([FromBody] RequestBase<OperacaoFilterDto?> request)
        => await _service.List(request.Data ?? new OperacaoFilterDto());

    [HttpPost("Get")]
    public async Task<SingleResponse<OperacaoDto>> Get([FromBody] RequestBase<int> request)
        => await _service.Get(request.Data);

    [HttpPost("Insert")]
    public async Task<SingleResponse<int>> Insert([FromBody] RequestBase<OperacaoDto> request)
        => await _service.Insert(request.Data);

    [HttpPost("Update")]
    public async Task<SingleResponse<bool>> Update([FromBody] RequestBase<OperacaoDto> request)
        => await _service.Update(request.Data);

    [HttpPost("Delete")]
    public async Task<SingleResponse<bool>> Delete([FromBody] RequestBase<int> request)
        => await _service.Delete(request.Data);
}
