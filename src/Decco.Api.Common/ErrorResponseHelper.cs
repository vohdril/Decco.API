using Decco.Contracts;

namespace Decco.Api.Common;

public static class ErrorResponseHelper
{
    public static SingleResponse<T> Fail<T>(string code, string message) => new()
    {
        Status = ResponseStatus.Fail,
        Error = new ErrorInfo { Code = code, Message = message }
    };

    public static SingleResponse<T> Fail<T>() =>
        Fail<T>("INTERNAL_ERROR", "Erro interno do servidor");

    public static SingleResponse<T> NotFound<T>() =>
        Fail<T>("NOT_FOUND", "Registro não encontrado");
}
