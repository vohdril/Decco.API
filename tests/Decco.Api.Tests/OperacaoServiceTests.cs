using Decco.Api.DataLayer.Models;
using Decco.Api.DataLayer.Repositories;
using Decco.Api.Services;
using Decco.Contracts;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace Decco.Api.Tests;

public class OperacaoServiceTests
{
    [Fact]
    public async Task List_passes_the_scoped_filter_to_the_repository()
    {
        OperacaoFilter? received = null;
        var repo = new Mock<IOperacaoRepository>();
        repo.Setup(r => r.SearchAsync(It.IsAny<OperacaoFilter>()))
            .Callback<OperacaoFilter>(f => received = f)
            .ReturnsAsync((new List<OperacaoSummary>
            {
                new() { Id = 1, Codigo = "OP-2026-0004", Codinome = "Sono Partilhado", TipoOperacaoCodigo = "INVESTIGACAO",
                        TipoOperacao = "Investigação", InstalacaoCodigo = "SITIO-64", Instalacao = "Sítio-64",
                        Objetivo = "x", Status = "PLANEJADA", Prioridade = 4, NivelAcessoMinimo = 2 }
            }, 51));

        var service = new OperacaoService(repo.Object, NullLogger<OperacaoService>.Instance);

        var response = await service.List(new OperacaoFilterDto
        {
            InstalacaoId = 64,
            IncluirSubinstalacoes = true,
            NivelAcessoUsuario = 2,
            PageIndex = 1,
            PageSize = 25
        });

        Assert.Equal(new OperacaoFilter(64, true, null, null, null, 2, 1, 25), received);
        Assert.Equal(ResponseStatus.Success, response.Status);
        Assert.Single(response.Data!);
        Assert.Equal("SITIO-64", response.Data![0].InstalacaoCodigo);
        Assert.Equal(51, response.TotalRecords);
        Assert.True(response.HasNextPage);   // (1 + 1) * 25 = 50 < 51
    }

    [Fact]
    public async Task List_with_a_repository_failure_returns_a_generic_error_without_leaking_the_exception()
    {
        var repo = new Mock<IOperacaoRepository>();
        repo.Setup(r => r.SearchAsync(It.IsAny<OperacaoFilter>()))
            .ThrowsAsync(new InvalidOperationException("Invalid object name 'Operacao'"));

        var service = new OperacaoService(repo.Object, NullLogger<OperacaoService>.Instance);

        var response = await service.List(new OperacaoFilterDto());

        Assert.Equal(ResponseStatus.Fail, response.Status);
        Assert.Equal("INTERNAL_ERROR", response.Error!.Code);
        Assert.DoesNotContain("Operacao", response.Error.Message);
    }
}
