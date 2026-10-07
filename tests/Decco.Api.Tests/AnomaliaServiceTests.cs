using Decco.Api.DataLayer.Models;
using Decco.Api.DataLayer.Repositories;
using Decco.Api.Services;
using Decco.Contracts;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace Decco.Api.Tests;

/// <summary>
/// Regression: AnomaliaDto only carried the classification NAMES, and MapToEntity copied
/// no Id at all. Every Insert reached sp_Anomalia_Inserir with ClasseObjetoId = 0 and
/// failed on the FK — with the error swallowed by a generic INTERNAL_ERROR. The test pins
/// down what reaches the repository.
/// </summary>
public class AnomaliaServiceTests
{
    [Fact]
    public async Task Insert_passes_the_classification_ids_and_the_facility_to_the_repository()
    {
        Anomalia? sent = null;
        var repo = new Mock<IAnomaliaRepository>();
        repo.Setup(r => r.InsertAsync(It.IsAny<Anomalia>()))
            .Callback<Anomalia>(a => sent = a)
            .ReturnsAsync(1002);

        var service = new AnomaliaService(repo.Object, NullLogger<AnomaliaService>.Instance);

        var response = await service.Insert(new AnomaliaDto
        {
            CodigoSCP = "SCP-1003",
            NomeComum = "Test",
            Descricao = "Test",
            ClasseObjetoId = 2,
            CamadaOntologicaId = 1,
            TipoMateriaId = 1,
            CognicaoAparenteId = 3,
            PericulosidadeId = 5,
            MecanismoPrimarioId = 7,
            MecanismoSecundarioId = 9,
            InstalacaoContencaoId = 4
        });

        Assert.Equal(ResponseStatus.Success, response.Status);
        Assert.Equal(1002, response.Data);
        Assert.NotNull(sent);
        Assert.Equal(2, sent!.ClasseObjetoId);
        Assert.Equal(1, sent.CamadaOntologicaId);
        Assert.Equal(1, sent.TipoMateriaId);
        Assert.Equal(3, sent.CognicaoAparenteId);
        Assert.Equal(5, sent.PericulosidadeId);
        Assert.Equal(7, sent.MecanismoPrimarioId);
        Assert.Equal(9, sent.MecanismoSecundarioId);
        Assert.Equal(4, sent.InstalacaoContencaoId);
    }

    [Fact]
    public async Task List_pages_in_the_repository_and_caps_the_page_size()
    {
        var repo = new Mock<IAnomaliaRepository>();
        repo.Setup(r => r.ListAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync((new List<Anomalia>(), 0));

        var service = new AnomaliaService(repo.Object, NullLogger<AnomaliaService>.Instance);

        var response = await service.List(page: -3, pageSize: 10_000);

        repo.Verify(r => r.ListAsync(0, 200), Times.Once);
        Assert.Equal(0, response.PageIndex);
        Assert.Equal(200, response.PageSize);
    }
}
