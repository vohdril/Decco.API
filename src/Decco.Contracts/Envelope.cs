namespace Decco.Contracts;

public class RequestBase<T>
{
    public T Data { get; set; } = default!;
    public string? Culture { get; set; }
}

/// <summary>
/// Pagination requested by the client — body of a List POST. 0-based, like the
/// PagedResponse that comes back. (Foundation.API exposes 1-based outside and converts.)
/// </summary>
public class PageRequest
{
    public int PageIndex { get; set; } = 0;
    public int PageSize { get; set; } = 50;
}

public class ResponseBase<T>
{
    public T? Data { get; set; }
    public ResponseStatus Status { get; set; } = ResponseStatus.Success;
    public ErrorInfo? Error { get; set; }
}

public class SingleResponse<T> : ResponseBase<T> { }

public class BulkResponse<T> : ResponseBase<List<T>> { }

public class PagedResponse<T> : ResponseBase<List<T>>
{
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public int TotalRecords { get; set; }
    public bool HasNextPage => (PageIndex + 1) * PageSize < TotalRecords;
}

public enum ResponseStatus
{
    Success,
    PartialSuccess,
    Fail
}

public class ErrorInfo
{
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
