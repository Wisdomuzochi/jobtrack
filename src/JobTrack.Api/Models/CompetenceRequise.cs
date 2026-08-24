namespace JobTrack.Api.Models;

public class CompetenceRequise
{
    public Guid Id { get; set; }
    public string Nom { get; set; } = string.Empty;

    // Clé étrangère : l'Id de la Candidature à laquelle cette compétence appartient.
    public Guid CandidatureId { get; set; }

    // Propriété de navigation : permet, depuis une CompetenceRequise, d'accéder
    // directement à l'objet Candidature complet (pas juste son Id). Optionnelle
    // pour EF Core, mais pratique pour naviguer dans le code.
    public Candidature? Candidature { get; set; }
}