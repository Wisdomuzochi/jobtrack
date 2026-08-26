using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using JobTrack.Api.Data;

namespace JobTrack.Api.Tests;

// Remplace la vraie base SQLite par une base en mémoire pour les tests
// d'intégration, exactement comme sur MiniDoc.
public class JobTrackWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _nomBaseDeTest = "JobTrackTestDb_" + Guid.NewGuid();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descripteur = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<JobTrackDbContext>));

            if (descripteur is not null)
            {
                services.Remove(descripteur);
            }

            services.AddDbContext<JobTrackDbContext>(options =>
                options.UseInMemoryDatabase(_nomBaseDeTest));
        });
    }
}