using Decco.Api.Services;
using Decco.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Decco.Api.REST.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CatTipoMateriumController : ControllerBase
{
    private readonly ICatTipoMateriumService _service;

    public CatTipoMateriumController(ICatTipoMateriumService service)
    {
        _service = service;
    }

    [HttpPost("List")]
    public async Task<SingleResponse<List<CatTipoMateriumDto>>> List([FromBody] RequestBase<object> request)
    {
        return await _service.List();
    }

    [HttpPost("Get")]
    public async Task<SingleResponse<CatTipoMateriumDto>> Get([FromBody] RequestBase<int> request)
    {
        return await _service.Get(request.Data);
    }

    [HttpPost("Insert")]
    public async Task<SingleResponse<int>> Insert([FromBody] RequestBase<CatTipoMateriumDto> request)
    {
        return await _service.Insert(request.Data);
    }

    [HttpPost("Update")]
    public async Task<SingleResponse<bool>> Update([FromBody] RequestBase<CatTipoMateriumDto> request)
    {
        return await _service.Update(request.Data);
    }

    [HttpPost("Delete")]
    public async Task<SingleResponse<bool>> Delete([FromBody] RequestBase<int> request)
    {
        return await _service.Delete(request.Data);
    }
}
