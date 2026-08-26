using JobTrack.Api.Data;
using JobTrack.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace JobTrack.Api.Services;

public class CandidatureService
{
    private readonly JobTrackDbContext _context;

    public CandidatureService(JobTrackDbContext context)
    {
        _context = context;
    }

    public Candidature CreerCandidature(
        string poste,
        string entreprise,
        string lienOffre,
        List<string> competences,
        List<Contact> contacts)
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
            Statut = CandidatureStatut.Envoyee,
            // Pas besoin de renseigner CandidatureId manuellement : EF Core le fait
            // automatiquement au SaveChanges(), en détectant que ces objets font
            // partie de la collection CompetencesRequises de cette candidature.
            CompetencesRequises = competences
                .Select(nom => new CompetenceRequise { Id = Guid.NewGuid(), Nom = nom })
                .ToList(),
            Contacts = contacts
        };

        _context.Candidatures.Add(candidature);
        _context.SaveChanges();

        return candidature;
    }
    
    public List<Candidature> ListerCandidatures()
    {
        // Sans Include, EF Core ne charge PAS les collections liées par défaut
        // (comportement volontaire, pour éviter de charger inutilement trop de
        // données à chaque requête). Include force le chargement de chaque
        // relation demandée, en une seule requête SQL optimisée (avec des JOIN).
        return _context.Candidatures
            .Include(c => c.CompetencesRequises)
            .Include(c => c.Contacts)
            .ToList();
    }
}