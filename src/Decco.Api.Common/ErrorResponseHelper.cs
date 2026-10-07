using System.Runtime.CompilerServices;
using Decco.Contracts;
using Microsoft.Extensions.Logging;

namespace Decco.Api.Common;

/// <summary>
/// Respostas de erro padronizadas do envelope.
///
/// REGRA DE SEGURANÇA: o cliente NUNCA recebe <c>ex.Message</c>. A mensagem de
/// erro é sempre genérica; o detalhe vai para o log, do lado de dentro.
///
/// As sobrecargas que recebem <see cref="ILogger"/> existem porque a versão
/// anterior deste helper era usada em <c>catch { }</c> sem log — o que tornou
/// o bug do typo "Anomalium" (Invalid object name) invisível por semanas.
/// Não logar não é segurança; é cegueira.
/// </summary>
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

    /// <summary>
    /// Registra a exceção e devolve o erro genérico.
    /// <paramref name="metodo"/> é preenchido pelo compilador com o nome do
    /// membro chamador (<see cref="CallerMemberNameAttribute"/>) — por isso o
    /// <c>catch</c> no serviço não precisa repetir o nome do método.
    /// </summary>
    public static SingleResponse<T> Fail<T>(
        ILogger logger,
        Exception ex,
        [CallerMemberName] string metodo = "")
    {
        logger.LogError(ex, "Falha inesperada em {Metodo} ({Retorno})", metodo, typeof(T).Name);
        return Fail<T>();
    }

    /// <summary>Variante paginada — mesmo contrato, para as operações de List.</summary>
    public static PagedResponse<T> FailPaged<T>(
        ILogger logger,
        Exception ex,
        [CallerMemberName] string metodo = "")
    {
        logger.LogError(ex, "Falha inesperada em {Metodo} (PagedResponse<{Retorno}>)", metodo, typeof(T).Name);
        return new PagedResponse<T>
        {
            Status = ResponseStatus.Fail,
            Error = new ErrorInfo { Code = "INTERNAL_ERROR", Message = "Erro interno do servidor" }
        };
    }
}
