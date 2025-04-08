using JobScraper.Console.Data;
using JobScraper.Console.Model;

public class ExclusionsService
{
    private readonly AppDbContext _dbContext;

    public ExclusionsService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddExclusion(string advertiser, string company)
    {
        var exclusion = new Exclusion
        {
            Advertiser = advertiser,
            Company = company
        };

        await _dbContext.AddAsync(exclusion);

        var advertiserJobs = _dbContext.Jobs
            .Where(x => x.Advertiser.ToLower() == advertiser.ToLower()
                && x.Company.ToLower() == company.ToLower())
            .ToList();
        _dbContext.RemoveRange(advertiserJobs);

        await _dbContext.SaveChangesAsync();
    }
}