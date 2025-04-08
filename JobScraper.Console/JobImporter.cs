using JobScraper.Console.Data;
using JobScraper.Console.Model;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

public class JobImporter 
{
    private readonly AppDbContext _dbContext;

    public JobImporter(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Import(string importsFolder)
    {
        var jobDtos = GetImportJobDtos(importsFolder);

        var existingJobs = await _dbContext.Jobs.ToListAsync();

        var newJobDtos = jobDtos.Where(x => !JobAlreadyExists(existingJobs, x)).ToList();
        var filteredJobDtos = await FilterOutExclusions(newJobDtos);

        var newJobs = filteredJobDtos.Select(Map);

        await _dbContext.Jobs.AddRangeAsync(newJobs);
        await _dbContext.SaveChangesAsync();
    }

    private async Task<List<JobImportDto>> FilterOutExclusions(List<JobImportDto> jobDtos)
    {
        var exclusions = await _dbContext.Exclusions.ToListAsync();

        return jobDtos.Where(dto => !ShouldExcludeJob(dto, exclusions)).ToList();
    }

    private bool ShouldExcludeJob(JobImportDto dto, List<Exclusion> exclusions)
        => exclusions.Any(x => x.Advertiser.ToLower() == dto.Advertiser.ToLower()
                && x.Company.ToLower() == dto.Company.ToLower());

    private List<JobImportDto> GetImportJobDtos(string importsFolder)
    {
        var filePaths = Directory.GetFiles(importsFolder);
        var jobDtos = filePaths.Select(File.ReadAllText)
            .Select(JsonConvert.DeserializeObject<List<JobImportDto>>)
            .SelectMany(x => x)
            .ToList();

        //filter jobs for unique that have advertiserId
        var uniqueByAdvertiser = jobDtos
            .Where(x => x.AdvertiserId != null)
            .DistinctBy(x => x.AdvertiserId);

        //put it all back together and filter for unique by url
        return jobDtos.Where(x => x.AdvertiserId == null)
            .Union(uniqueByAdvertiser)
            .DistinctBy(x => x.Url)
            .ToList();
    }

    private bool JobAlreadyExists(List<Job> existingJobs, JobImportDto newJob)
    {
        if (newJob.AdvertiserId != null && existingJobs.Any(x => x.AdvertiserId == newJob.AdvertiserId))
            return true;

        return existingJobs.Any(x => x.Url == newJob.Url);
    }

    private Job Map(JobImportDto dto) => new Job
    {
        Advertiser = dto.Advertiser,
        AdvertiserId = dto.AdvertiserId,
        Title = dto.Title,
        Company = dto.Company,
        Url = dto.Url,
        DateFound = DateTime.Now,
        Status = JobStatus.New
    };

    private class JobImportDto
    {
        public string Advertiser { get; set; }
        public string AdvertiserId { get; set; }
        public string Title { get; set; }
        public string Company { get; set; }
        public string Url { get; set; }

    }
}
