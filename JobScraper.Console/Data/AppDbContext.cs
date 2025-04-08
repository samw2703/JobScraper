using JobScraper.Console.Model;
using Microsoft.EntityFrameworkCore;

namespace JobScraper.Console.Data;


public class AppDbContext : DbContext
{
    private readonly string _dbPath;

    public AppDbContext(string dataFolder)
    {
        _dbPath = $"{dataFolder}/Data/Jobs.db";
    }

    public DbSet<Job> Jobs => Set<Job>();
    public DbSet<Exclusion> Exclusions => Set<Exclusion>();

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={_dbPath}");
}