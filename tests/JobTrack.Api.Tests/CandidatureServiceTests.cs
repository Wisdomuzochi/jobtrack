using JobTrack.Api.Data;
using JobTrack.Api.Models;
using JobTrack.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace JobTrack.Api.Tests;

public class CandidatureServiceTests
{
    // Crée un DbContext connecté à une base en mémoire, unique à chaque appel,
    private static JobTrackDbContext CreerContexteDeTest()
    {
        var options = new DbContextOptionsBuilder<JobTrackDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new JobTrackDbContext(options);
    }

    [Fact]
    public void CreerCandidature_AvecDonneesValides_RetourneCandidatureAvecStatutEnvoyee()
    {
        // Arrange
        var context = CreerContexteDeTest();
        var service = new CandidatureService(context);

        // Act
        var candidature = service.CreerCandidature("Développeur .NET", "MAF", "https://...");

        // Assert
        Assert.Equal("Développeur .NET", candidature.Poste);
        Assert.Equal("MAF", candidature.Entreprise);
        Assert.Equal(CandidatureStatut.Envoyee, candidature.Statut);
        Assert.NotEqual(Guid.Empty, candidature.Id);
    }

    [Fact]
    public void CreerCandidature_AvecPosteVide_LeveUneException()
    {
        // Arrange
        var context = CreerContexteDeTest();
        var service = new CandidatureService(context);

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            service.CreerCandidature("", "MAF", "https://..."));
    }
}