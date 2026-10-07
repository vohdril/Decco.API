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
    public async Task List_repassa_o_filtro_escopado_ao_repositorio()
    {
        OperacaoFiltro? recebido = null;
        var repo = new Mock<IOperacaoRepository>();
        repo.Setup(r => r.BuscarAsync(It.IsAny<OperacaoFiltro>()))
            .Callback<OperacaoFiltro>(f => recebido = f)
            .ReturnsAsync((new List<OperacaoResumo>
            {
                new() { Id = 1, Codigo = "OP-2026-0004", Codinome = "Sono Partilhado", TipoOperacaoCodigo = "INVESTIGACAO",
                        TipoOperacao = "Investigação", InstalacaoCodigo = "SITIO-64", Instalacao = "Sítio-64",
                        Objetivo = "x", Status = "PLANEJADA", Prioridade = 4, NivelAcessoMinimo = 2 }
            }, 51));

        var service = new OperacaoService(repo.Object, NullLogger<OperacaoService>.Instance);

        var resposta = await service.List(new OperacaoFiltroDto
        {
            InstalacaoId = 64,
            IncluirSubinstalacoes = true,
            NivelAcessoUsuario = 2,
            PageIndex = 1,
            PageSize = 25
        });

        Assert.Equal(new OperacaoFiltro(64, true, null, null, null, 2, 1, 25), recebido);
        Assert.Equal(ResponseStatus.Success, resposta.Status);
        Assert.Single(resposta.Data!);
        Assert.Equal("SITIO-64", resposta.Data![0].InstalacaoCodigo);
        Assert.Equal(51, resposta.TotalRecords);
        Assert.True(resposta.HasNextPage);   // (1 + 1) * 25 = 50 < 51
    }

    [Fact]
    public async Task List_com_falha_no_repositorio_devolve_erro_generico_sem_vazar_a_excecao()
    {
        var repo = new Mock<IOperacaoRepository>();
        repo.Setup(r => r.BuscarAsync(It.IsAny<OperacaoFiltro>()))
            .ThrowsAsync(new InvalidOperationException("Invalid object name 'Operacao'"));

        var service = new OperacaoService(repo.Object, NullLogger<OperacaoService>.Instance);

        var resposta = await service.List(new OperacaoFiltroDto());

        Assert.Equal(ResponseStatus.Fail, resposta.Status);
        Assert.Equal("INTERNAL_ERROR", resposta.Error!.Code);
        Assert.DoesNotContain("Operacao", resposta.Error.Message);
    }
}
