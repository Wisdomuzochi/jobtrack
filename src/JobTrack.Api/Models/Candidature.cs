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

    public List<CompetenceRequise> CompetencesRequises { get; set; } = new();
    public List<Contact> Contacts { get; set; } = new();

    // Propriété calculée (pas stockée en base) : recalculée à chaque accès,
    // toujours à jour par rapport à la date actuelle.
    public bool RelanceConseillee =>
        Statut == CandidatureStatut.Envoyee &&
        (DateTime.UtcNow - DateCandidature).TotalDays > 15;
}