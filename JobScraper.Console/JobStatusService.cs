using JobScraper.Console.Data;
using JobScraper.Console.Model;
using Microsoft.EntityFrameworkCore;

public class JobStatusService
{
    private readonly AppDbContext _dbContext;

    public JobStatusService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task UpdateJobStatus(List<int> jobIds, JobStatus jobStatus)
    {
        var jobs = await _dbContext.Jobs.Where(x => jobIds.Contains(x.Id)).ToListAsync();

        jobs.ForEach(x => x.Status = jobStatus);

        await _dbContext.SaveChangesAsync();
    }
}
