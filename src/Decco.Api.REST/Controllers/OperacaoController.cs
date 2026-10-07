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

    // O corpo do List é o FILTRO (OperacaoFiltroDto) — é por isso que a API é POST:
    // o contrato descreve o recorte (instalação, tipo, status, clearance, página).
    [HttpPost("List")]
    public async Task<PagedResponse<OperacaoDto>> List([FromBody] RequestBase<OperacaoFiltroDto?> request)
        => await _service.List(request.Data ?? new OperacaoFiltroDto());

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
