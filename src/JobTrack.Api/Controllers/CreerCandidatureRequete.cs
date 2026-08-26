using Microsoft.AspNetCore.Mvc;
using JobTrack.Api.Models;
using JobTrack.Api.Services;

namespace JobTrack.Api.Controllers;

// DTO : représente exactement ce que le client a le droit d'envoyer pour
// créer une candidature. On ne laisse jamais le client fixer l'Id ou le Statut.
public class CreerCandidatureRequete
{
    public string Poste { get; set; } = string.Empty;
    public string Entreprise { get; set; } = string.Empty;
    public string LienOffre { get; set; } = string.Empty;
    public List<string> Competences { get; set; } = new();
    public List<Contact> Contacts { get; set; } = new();
}

[ApiController]
[Route("api/candidatures")]
public class CandidaturesController : ControllerBase
{
    private readonly CandidatureService _candidatureService;

    public CandidaturesController(CandidatureService candidatureService)
    {
        _candidatureService = candidatureService;
    }

    // Répond à : POST /api/candidatures
    [HttpPost]
    public IActionResult CreerCandidature([FromBody] CreerCandidatureRequete requete)
    {
        try
        {
            var candidature = _candidatureService.CreerCandidature(
                requete.Poste,
                requete.Entreprise,
                requete.LienOffre,
                requete.Competences,
                requete.Contacts
            );

            return Created($"/api/candidatures/{candidature.Id}", candidature);
        }
        catch (ArgumentException)
        {
            return BadRequest();
        }
    }

    // Répond à : GET /api/candidatures
    [HttpGet]
    public IActionResult ListerCandidatures()
    {
        var candidatures = _candidatureService.ListerCandidatures();
        return Ok(candidatures);
    }
}