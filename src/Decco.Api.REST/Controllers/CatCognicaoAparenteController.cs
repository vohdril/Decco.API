using Decco.Api.Services;
using Decco.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Decco.Api.REST.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CatCognicaoAparenteController : ControllerBase
{
    private readonly ICatCognicaoAparenteService _service;

    public CatCognicaoAparenteController(ICatCognicaoAparenteService service)
    {
        _service = service;
    }

    [HttpPost("List")]
    public async Task<SingleResponse<List<CatCognicaoAparenteDto>>> List([FromBody] RequestBase<object> request)
    {
        return await _service.List();
    }

    [HttpPost("Get")]
    public async Task<SingleResponse<CatCognicaoAparenteDto>> Get([FromBody] RequestBase<int> request)
    {
        return await _service.Get(request.Data);
    }

    [HttpPost("Insert")]
    public async Task<SingleResponse<int>> Insert([FromBody] RequestBase<CatCognicaoAparenteDto> request)
    {
        return await _service.Insert(request.Data);
    }

    [HttpPost("Update")]
    public async Task<SingleResponse<bool>> Update([FromBody] RequestBase<CatCognicaoAparenteDto> request)
    {
        return await _service.Update(request.Data);
    }

    [HttpPost("Delete")]
    public async Task<SingleResponse<bool>> Delete([FromBody] RequestBase<int> request)
    {
        return await _service.Delete(request.Data);
    }
}
