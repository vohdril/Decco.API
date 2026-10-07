using System.Runtime.CompilerServices;
using Decco.Contracts;
using Microsoft.Extensions.Logging;

namespace Decco.Api.Common;

/// <summary>
/// Standardized envelope error responses.
///
/// SECURITY RULE: the client NEVER receives <c>ex.Message</c>. The error message is always
/// generic; the details go to the log, on the inside.
///
/// The overloads that take an <see cref="ILogger"/> exist because the previous version of
/// this helper was used in <c>catch { }</c> blocks without logging — which made the
/// "Anomalium" typo bug (Invalid object name) invisible for weeks.
/// Not logging is not security; it is blindness.
/// </summary>
public static class ErrorResponseHelper
{
    public static SingleResponse<T> Fail<T>(string code, string message) => new()
    {
        Status = ResponseStatus.Fail,
        Error = new ErrorInfo { Code = code, Message = message }
    };

    public static SingleResponse<T> Fail<T>() =>
        Fail<T>("INTERNAL_ERROR", "Internal server error");

    public static SingleResponse<T> NotFound<T>() =>
        Fail<T>("NOT_FOUND", "Record not found");

    /// <summary>
    /// Logs the exception and returns the generic error.
    /// <paramref name="method"/> is filled by the compiler with the caller member name
    /// (<see cref="CallerMemberNameAttribute"/>) — so the service's <c>catch</c> does not
    /// need to repeat the method name.
    /// </summary>
    public static SingleResponse<T> Fail<T>(
        ILogger logger,
        Exception ex,
        [CallerMemberName] string method = "")
    {
        logger.LogError(ex, "Unexpected failure in {Method} ({ReturnType})", method, typeof(T).Name);
        return Fail<T>();
    }

    /// <summary>Paged variant — same contract, for List operations.</summary>
    public static PagedResponse<T> FailPaged<T>(
        ILogger logger,
        Exception ex,
        [CallerMemberName] string method = "")
    {
        logger.LogError(ex, "Unexpected failure in {Method} (PagedResponse<{ReturnType}>)", method, typeof(T).Name);
        return new PagedResponse<T>
        {
            Status = ResponseStatus.Fail,
            Error = new ErrorInfo { Code = "INTERNAL_ERROR", Message = "Internal server error" }
        };
    }
}
