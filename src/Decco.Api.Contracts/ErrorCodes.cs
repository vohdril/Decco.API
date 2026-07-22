namespace Decco.Api.Contracts;

public static class ErrorCodes
{
    public static readonly ErrorCode NotFound = new("NOT_FOUND", "Registro não encontrado");
    public static readonly ErrorCode ValidationFailed = new("VALIDATION_FAILED", "Falha na validação");
    public static readonly ErrorCode InternalError = new("INTERNAL_ERROR", "Erro interno do servidor");
}

public class ErrorCode
{
    public string Code { get; }
    public string DefaultMessage { get; }

    public ErrorCode(string code, string defaultMessage)
    {
        Code = code;
        DefaultMessage = defaultMessage;
    }

    public string GetCode() => Code;
}
