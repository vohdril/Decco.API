using Decco.Api.DataLayer.Models;
using Decco.Api.DataLayer.Repositories;
using Decco.Api.Services;
using Decco.Contracts;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace Decco.Api.Tests;

/// <summary>
/// Regressão: o AnomaliaDto só carregava os NOMES da classificação, e o
/// MapToEntity não copiava nenhum Id. Todo Insert chegava à sp_Anomalia_Inserir
/// com ClasseObjetoId = 0 e falhava na FK — com o erro engolido num
/// INTERNAL_ERROR genérico. O teste prende o que vai para o repositório.
/// </summary>
public class AnomaliaServiceTests
{
    [Fact]
    public async Task Insert_repassa_os_ids_de_classificacao_e_a_instalacao_ao_repositorio()
    {
        Anomalia? enviada = null;
        var repo = new Mock<IAnomaliaRepository>();
        repo.Setup(r => r.InsertAsync(It.IsAny<Anomalia>()))
            .Callback<Anomalia>(a => enviada = a)
            .ReturnsAsync(1002);

        var service = new AnomaliaService(repo.Object, NullLogger<AnomaliaService>.Instance);

        var resposta = await service.Insert(new AnomaliaDto
        {
            CodigoSCP = "SCP-1003",
            NomeComum = "Teste",
            Descricao = "Teste",
            ClasseObjetoId = 2,
            CamadaOntologicaId = 1,
            TipoMateriaId = 1,
            CognicaoAparenteId = 3,
            PericulosidadeId = 5,
            MecanismoPrimarioId = 7,
            MecanismoSecundarioId = 9,
            InstalacaoContencaoId = 4
        });

        Assert.Equal(ResponseStatus.Success, resposta.Status);
        Assert.Equal(1002, resposta.Data);
        Assert.NotNull(enviada);
        Assert.Equal(2, enviada!.ClasseObjetoId);
        Assert.Equal(1, enviada.CamadaOntologicaId);
        Assert.Equal(1, enviada.TipoMateriaId);
        Assert.Equal(3, enviada.CognicaoAparenteId);
        Assert.Equal(5, enviada.PericulosidadeId);
        Assert.Equal(7, enviada.MecanismoPrimarioId);
        Assert.Equal(9, enviada.MecanismoSecundarioId);
        Assert.Equal(4, enviada.InstalacaoContencaoId);
    }

    [Fact]
    public async Task List_pagina_no_repositorio_e_limita_o_tamanho_da_pagina()
    {
        var repo = new Mock<IAnomaliaRepository>();
        repo.Setup(r => r.ListAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync((new List<Anomalia>(), 0));

        var service = new AnomaliaService(repo.Object, NullLogger<AnomaliaService>.Instance);

        var resposta = await service.List(page: -3, pageSize: 10_000);

        repo.Verify(r => r.ListAsync(0, 200), Times.Once);
        Assert.Equal(0, resposta.PageIndex);
        Assert.Equal(200, resposta.PageSize);
    }
}
