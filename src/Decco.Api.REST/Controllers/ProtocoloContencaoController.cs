using Decco.Api.Services;
using Decco.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Decco.Api.REST.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProtocoloContencaoController : ControllerBase
{
    private readonly IProtocoloContencaoService _service;

    public ProtocoloContencaoController(IProtocoloContencaoService service)
    {
        _service = service;
    }

    [HttpPost("List")]
    public async Task<SingleResponse<List<ProtocoloContencaoDto>>> List([FromBody] RequestBase<object> request)
        => await _service.List();

    [HttpPost("Get")]
    public async Task<SingleResponse<ProtocoloContencaoDto>> Get([FromBody] RequestBase<int> request)
        => await _service.Get(request.Data);

    [HttpPost("Insert")]
    public async Task<SingleResponse<int>> Insert([FromBody] RequestBase<ProtocoloContencaoDto> request)
        => await _service.Insert(request.Data);

    [HttpPost("Update")]
    public async Task<SingleResponse<bool>> Update([FromBody] RequestBase<ProtocoloContencaoDto> request)
        => await _service.Update(request.Data);

    [HttpPost("Delete")]
    public async Task<SingleResponse<bool>> Delete([FromBody] RequestBase<int> request)
        => await _service.Delete(request.Data);
}
