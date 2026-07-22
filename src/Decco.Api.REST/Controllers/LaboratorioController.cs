using Decco.Api.Services;
using Decco.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Decco.Api.REST.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LaboratorioController : ControllerBase
{
    private readonly ILaboratorioService _service;

    public LaboratorioController(ILaboratorioService service)
    {
        _service = service;
    }

    [HttpPost("List")]
    public async Task<SingleResponse<List<LaboratorioDto>>> List([FromBody] RequestBase<object> request)
        => await _service.List();

    [HttpPost("Get")]
    public async Task<SingleResponse<LaboratorioDto>> Get([FromBody] RequestBase<int> request)
        => await _service.Get(request.Data);

    [HttpPost("Insert")]
    public async Task<SingleResponse<int>> Insert([FromBody] RequestBase<LaboratorioDto> request)
        => await _service.Insert(request.Data);

    [HttpPost("Update")]
    public async Task<SingleResponse<bool>> Update([FromBody] RequestBase<LaboratorioDto> request)
        => await _service.Update(request.Data);

    [HttpPost("Delete")]
    public async Task<SingleResponse<bool>> Delete([FromBody] RequestBase<int> request)
        => await _service.Delete(request.Data);
}
