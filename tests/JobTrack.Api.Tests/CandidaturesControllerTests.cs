using System.Net;
using System.Net.Http.Json;
using JobTrack.Api.Models;

namespace JobTrack.Api.Tests;

public class CandidaturesControllerTests : IClassFixture<JobTrackWebApplicationFactory>
{
    private readonly HttpClient _client;

    public CandidaturesControllerTests(JobTrackWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PostCandidatures_AvecDonneesValides_Retourne201()
    {
        // Arrange
        var requete = new
        {
            Poste = "Développeur .NET",
            Entreprise = "MAF",
            LienOffre = "https://...",
            Competences = new List<string> { "C#", "EF Core" },
            Contacts = new List<Contact>()
        };

        // Act
        var reponse = await _client.PostAsJsonAsync("/api/candidatures", requete);

        // Assert
        Assert.Equal(HttpStatusCode.Created, reponse.StatusCode);

        var candidature = await reponse.Content.ReadFromJsonAsync<Candidature>();
        Assert.NotNull(candidature);
        Assert.Equal("Développeur .NET", candidature!.Poste);
        Assert.Equal(2, candidature.CompetencesRequises.Count);
    }

    [Fact]
    public async Task PostCandidatures_AvecPosteVide_Retourne400()
    {
        // Arrange
        var requete = new
        {
            Poste = "",
            Entreprise = "MAF",
            LienOffre = "https://...",
            Competences = new List<string>(),
            Contacts = new List<Contact>()
        };

        // Act
        var reponse = await _client.PostAsJsonAsync("/api/candidatures", requete);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, reponse.StatusCode);
    }

    [Fact]
    public async Task GetCandidatures_ApresUnPost_RetourneLaCandidature()
    {
        // Arrange
        var requete = new
        {
            Poste = "Ingénieur Logiciel",
            Entreprise = "Netflix",
            LienOffre = "https://...",
            Competences = new List<string> { "Java" },
            Contacts = new List<Contact>()
        };
        await _client.PostAsJsonAsync("/api/candidatures", requete);

        // Act
        var reponse = await _client.GetAsync("/api/candidatures");

        // Assert
        Assert.Equal(HttpStatusCode.OK, reponse.StatusCode);
        var candidatures = await reponse.Content.ReadFromJsonAsync<List<Candidature>>();
        Assert.NotNull(candidatures);
        Assert.Contains(candidatures!, c => c.Poste == "Ingénieur Logiciel");
    }

    [Fact]
    public async Task PutCandidatures_AvecIdExistant_ChangeLeStatut()
    {
        var creation = new
        {
            Poste = "Testeur", Entreprise = "Corp", LienOffre = "https://...",
            Competences = new List<string>(), Contacts = new List<Contact>()
        };
        var reponseCreation = await _client.PostAsJsonAsync("/api/candidatures", creation);
        var candidatureCreee = await reponseCreation.Content.ReadFromJsonAsync<Candidature>();

        var requeteModif = new { NouveauStatut = CandidatureStatut.EnCours };

        var reponse = await _client.PutAsJsonAsync($"/api/candidatures/{candidatureCreee!.Id}", requeteModif);

        Assert.Equal(HttpStatusCode.OK, reponse.StatusCode);
        var candidatureModifiee = await reponse.Content.ReadFromJsonAsync<Candidature>();
        Assert.Equal(CandidatureStatut.EnCours, candidatureModifiee!.Statut);
    }

    [Fact]
    public async Task PutCandidatures_AvecIdInexistant_Retourne404()
    {
        var requeteModif = new { NouveauStatut = CandidatureStatut.EnCours };

        var reponse = await _client.PutAsJsonAsync($"/api/candidatures/{Guid.NewGuid()}", requeteModif);

        Assert.Equal(HttpStatusCode.NotFound, reponse.StatusCode);
    }

    [Fact]
    public async Task DeleteCandidatures_AvecIdExistant_Retourne204()
    {
        var creation = new
        {
            Poste = "Testeur", Entreprise = "Corp", LienOffre = "https://...",
            Competences = new List<string>(), Contacts = new List<Contact>()
        };
        var reponseCreation = await _client.PostAsJsonAsync("/api/candidatures", creation);
        var candidatureCreee = await reponseCreation.Content.ReadFromJsonAsync<Candidature>();

        var reponse = await _client.DeleteAsync($"/api/candidatures/{candidatureCreee!.Id}");

        Assert.Equal(HttpStatusCode.NoContent, reponse.StatusCode);
    }

    [Fact]
    public async Task DeleteCandidatures_AvecIdInexistant_Retourne404()
    {
        var reponse = await _client.DeleteAsync($"/api/candidatures/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, reponse.StatusCode);
    }
}