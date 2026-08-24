namespace JobTrack.Api.Models;

public class Contact
{
    public Guid Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string LienLinkedIn { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;

    public Guid CandidatureId { get; set; }
    public Candidature? Candidature { get; set; }
}