using JobTrack.Api.Data;
using JobTrack.Api.Models;

namespace JobTrack.Api.Services;

public class CandidatureService
{
    private readonly JobTrackDbContext _context;

    public CandidatureService(JobTrackDbContext context)
    {
        _context = context;
    }

    public Candidature CreerCandidature(string poste, string entreprise, string lienOffre)
    {
        if (string.IsNullOrWhiteSpace(poste))
        {
            throw new ArgumentException("Le poste est obligatoire.", nameof(poste));
        }

        var candidature = new Candidature
        {
            Id = Guid.NewGuid(),
            Poste = poste,
            Entreprise = entreprise,
            LienOffre = lienOffre,
            DateCandidature = DateTime.UtcNow,
            Statut = CandidatureStatut.Envoyee
        };

        _context.Candidatures.Add(candidature);
        _context.SaveChanges();

        return candidature;
    }
}