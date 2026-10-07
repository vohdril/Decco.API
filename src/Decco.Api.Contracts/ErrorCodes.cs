namespace Decco.Api.Contracts;

public static class ErrorCodes
{
    public static readonly ErrorCode NotFound = new("NOT_FOUND", "Record not found");
    public static readonly ErrorCode ValidationFailed = new("VALIDATION_FAILED", "Validation failed");
    public static readonly ErrorCode InternalError = new("INTERNAL_ERROR", "Internal server error");
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
