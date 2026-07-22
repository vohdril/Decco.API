using Decco.Api.Services;
using Decco.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Decco.Api.REST.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CatMecanismoInteracaoController : ControllerBase
{
    private readonly ICatMecanismoInteracaoService _service;

    public CatMecanismoInteracaoController(ICatMecanismoInteracaoService service)
    {
        _service = service;
    }

    [HttpPost("List")]
    public async Task<SingleResponse<List<CatMecanismoInteracaoDto>>> List([FromBody] RequestBase<object> request)
    {
        return await _service.List();
    }

    [HttpPost("Get")]
    public async Task<SingleResponse<CatMecanismoInteracaoDto>> Get([FromBody] RequestBase<int> request)
    {
        return await _service.Get(request.Data);
    }

    [HttpPost("Insert")]
    public async Task<SingleResponse<int>> Insert([FromBody] RequestBase<CatMecanismoInteracaoDto> request)
    {
        return await _service.Insert(request.Data);
    }

    [HttpPost("Update")]
    public async Task<SingleResponse<bool>> Update([FromBody] RequestBase<CatMecanismoInteracaoDto> request)
    {
        return await _service.Update(request.Data);
    }

    [HttpPost("Delete")]
    public async Task<SingleResponse<bool>> Delete([FromBody] RequestBase<int> request)
    {
        return await _service.Delete(request.Data);
    }
}
