using JobTrack.Api.Data;
using JobTrack.Api.Models;
using JobTrack.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace JobTrack.Api.Tests;

public class CandidatureServiceTests
{
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
        var candidature = service.CreerCandidature(
            "Développeur .NET", "MAF", "https://...",
            new List<string>(), new List<Contact>());

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
            service.CreerCandidature(
                "", "MAF", "https://...",
                new List<string>(), new List<Contact>()));
    }

    [Fact]
    public void CreerCandidature_AvecCompetencesEtContacts_LesLieCorrectement()
    {
        // Arrange
        var context = CreerContexteDeTest();
        var service = new CandidatureService(context);

        var competences = new List<string> { "C#", "Angular", "SQL Server" };
        var contacts = new List<Contact>
        {
            new Contact { Nom = "Jean Dupont", Email = "jean@exemple.com", Role = "RH" }
        };

        // Act
        var candidature = service.CreerCandidature(
            "Développeur .NET", "MAF", "https://...", competences, contacts);

        // Assert
        Assert.Equal(3, candidature.CompetencesRequises.Count);
        Assert.Equal("C#", candidature.CompetencesRequises[0].Nom);
        Assert.Single(candidature.Contacts);
        Assert.Equal("Jean Dupont", candidature.Contacts[0].Nom);
    }

    [Fact]
    public void ListerCandidatures_ChargeLesCompetencesEtContactsLies()
    {
        // Arrange
        var context = CreerContexteDeTest();
        var service = new CandidatureService(context);

        service.CreerCandidature(
            "Développeur .NET", "MAF", "https://...",
             new List<string> { "C#", "EF Core" },
             new List<Contact> { new Contact { Nom = "Marie Curie", Email = "marie@exemple.com" } });

        // Act
        var candidatures = service.ListerCandidatures();

        // Assert
        Assert.Single(candidatures);
        Assert.Equal(2, candidatures[0].CompetencesRequises.Count);
        Assert.Single(candidatures[0].Contacts);
    }
}