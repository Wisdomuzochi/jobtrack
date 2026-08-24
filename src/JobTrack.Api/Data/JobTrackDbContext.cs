using Microsoft.EntityFrameworkCore;
using JobTrack.Api.Models;

namespace JobTrack.Api.Data;

public class JobTrackDbContext : DbContext
{
    public JobTrackDbContext(DbContextOptions<JobTrackDbContext> options) : base(options)
    {
    }

    public DbSet<Candidature> Candidatures { get; set; }
    public DbSet<CompetenceRequise> CompetencesRequises { get; set; }
    public DbSet<Contact> Contacts { get; set; }
}