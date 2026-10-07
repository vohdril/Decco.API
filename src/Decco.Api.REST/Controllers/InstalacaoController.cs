using Decco.Api.Services;
using Decco.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Decco.Api.REST.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InstalacaoController : ControllerBase
{
    private readonly IInstalacaoService _service;

    public InstalacaoController(IInstalacaoService service)
    {
        _service = service;
    }

    [HttpPost("List")]
    public async Task<SingleResponse<List<InstalacaoDto>>> List([FromBody] RequestBase<object> request)
        => await _service.List();

    [HttpPost("Get")]
    public async Task<SingleResponse<InstalacaoDto>> Get([FromBody] RequestBase<int> request)
        => await _service.Get(request.Data);

    [HttpPost("Insert")]
    public async Task<SingleResponse<int>> Insert([FromBody] RequestBase<InstalacaoDto> request)
        => await _service.Insert(request.Data);

    [HttpPost("Update")]
    public async Task<SingleResponse<bool>> Update([FromBody] RequestBase<InstalacaoDto> request)
        => await _service.Update(request.Data);

    [HttpPost("Delete")]
    public async Task<SingleResponse<bool>> Delete([FromBody] RequestBase<int> request)
        => await _service.Delete(request.Data);
}
