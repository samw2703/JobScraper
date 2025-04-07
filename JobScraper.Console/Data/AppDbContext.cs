using JobScraper.Console.Model;
using Microsoft.EntityFrameworkCore;

namespace JobScraper.Console.Data;


public class AppDbContext : DbContext
{
    public DbSet<SearchTerm> SearchTerms => Set<SearchTerm>();
    public DbSet<Area> Areas => Set<Area>();
    public DbSet<Job> Jobs => Set<Job>();

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite("Data Source=app.db");
}