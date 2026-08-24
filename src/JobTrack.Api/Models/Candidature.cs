namespace JobTrack.Api.Models;

public class Candidature
{
    public Guid Id { get; set; }

    public string Poste { get; set; } = string.Empty;
    public string Entreprise { get; set; } = string.Empty;
    public string LienOffre { get; set; } = string.Empty;

    public DateTime? DatePublicationOffre { get; set; }
    public DateTime DateCandidature { get; set; }

    public CandidatureStatut Statut { get; set; }

    public string Notes { get; set; } = string.Empty;

    // Ces deux collections seront remplies au Ticket #2, une fois les
    // relations EF Core mises en place. On les déclare dès maintenant
    // pour que la classe soit prête, mais vides pour l'instant.
    public List<CompetenceRequise> CompetencesRequises { get; set; } = new();
    public List<Contact> Contacts { get; set; } = new();
}