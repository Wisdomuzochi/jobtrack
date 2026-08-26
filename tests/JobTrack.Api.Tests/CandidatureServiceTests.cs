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

    [Fact]
     public void ChangerStatut_AvecIdExistant_MetAJourLeStatut()
    {
        var context = CreerContexteDeTest();
        var service = new CandidatureService(context);
        var candidature = service.CreerCandidature(
            "Développeur .NET", "MAF", "https://...", new List<string>(), new List<Contact>());

        var candidatureModifiee = service.ChangerStatut(candidature.Id, CandidatureStatut.EnCours);

        Assert.Equal(CandidatureStatut.EnCours, candidatureModifiee!.Statut);
}

    [Fact]
    public void ChangerStatut_AvecIdInexistant_RetourneNull()
    {
        var context = CreerContexteDeTest();
        var service = new CandidatureService(context);

        var resultat = service.ChangerStatut(Guid.NewGuid(), CandidatureStatut.EnCours);

        Assert.Null(resultat);
    }

    [Fact]
    public void SupprimerCandidature_AvecIdExistant_RetourneTrue()
    {
        var context = CreerContexteDeTest();
        var service = new CandidatureService(context);
        var candidature = service.CreerCandidature(
            "Développeur .NET", "MAF", "https://...", new List<string>(), new List<Contact>());

        var resultat = service.SupprimerCandidature(candidature.Id);

        Assert.True(resultat);
        Assert.Empty(service.ListerCandidatures());
    }

    [Fact]
    public void SupprimerCandidature_AvecIdInexistant_RetourneFalse()
    {
        var context = CreerContexteDeTest();
        var service = new CandidatureService(context);

        var resultat = service.SupprimerCandidature(Guid.NewGuid());

        Assert.False(resultat);
    }

    [Fact]
    public void RelanceConseillee_CandidatureEnvoyeeDepuisPlusDe15Jours_RetourneTrue()
    {
        // Arrange
        var candidature = new Candidature
        {
            Statut = CandidatureStatut.Envoyee,
            DateCandidature = DateTime.UtcNow.AddDays(-20)
        };

        // Assert
        Assert.True(candidature.RelanceConseillee);
    }

    [Fact]
    public void RelanceConseillee_CandidatureEnvoyeeRecemment_RetourneFalse()
    {
        // Arrange
        var candidature = new Candidature
        {
            Statut = CandidatureStatut.Envoyee,
            DateCandidature = DateTime.UtcNow.AddDays(-2)
        };

        // Assert
        Assert.False(candidature.RelanceConseillee);
    }

    [Fact]
    public void RelanceConseillee_CandidatureEnCoursDepuisLongtemps_RetourneFalse()
    {
        // Arrange
        var candidature = new Candidature
        {
           Statut = CandidatureStatut.EnCours,
           DateCandidature = DateTime.UtcNow.AddDays(-20)
        };

        // Assert
        Assert.False(candidature.RelanceConseillee);
    }
}