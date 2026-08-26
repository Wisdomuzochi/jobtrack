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

public class ChangerStatutRequete
{
    public CandidatureStatut NouveauStatut { get; set; }
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

    // Répond à : PUT /api/candidatures/{id}
    [HttpPut("{id}")]
    public IActionResult ChangerStatut(Guid id, [FromBody] ChangerStatutRequete requete)
    {
        var candidature = _candidatureService.ChangerStatut(id, requete.NouveauStatut);

        if (candidature is null)
        {
            return NotFound();
        }

        return Ok(candidature);
    }

    // Répond à : DELETE /api/candidatures/{id}
    [HttpDelete("{id}")]
    public IActionResult SupprimerCandidature(Guid id)
    {
        var succes = _candidatureService.SupprimerCandidature(id);

        if (!succes)
        {
            return NotFound();
        }

        return NoContent();
    }
}