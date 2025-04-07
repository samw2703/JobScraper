using JobScraper.Console.Data;
using JobScraper.Console.Model;

public class JobExporter
{
    private readonly AppDbContext _dbContext;

    public JobExporter(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task ExportNew(string exportsFolder)
    {
        var newJobs = _dbContext.Jobs.Where(x => x.Status == JobStatus.New).ToList();
        var csv = ToCsv(newJobs);

        File.WriteAllText($"{exportsFolder}/new_{DateTime.Now:yyyy-MM-dd_HH-mm}.csv", csv);
    }

    private string CreateFilePath(string exportsFolder, JobStatus jobStatus)
        => $"{exportsFolder}/new{jobStatus.ToString().ToLower()}_{DateTime.Now:yyyy-MM-dd_HH-mm}.csv";

    private string ToCsv(List<Job> jobs)
    {
        var csvRows = new List<string>()
        {
            "Id,Company,Title,Url,Date Found,Advertiser"
        };

        csvRows.AddRange(jobs.Select(ToCsvRow));
        return string.Join(Environment.NewLine, csvRows);
    }

    private string ToCsvRow(Job job)
        => $"\"{job.Id}\",\"{job.Company}\",\"{job.Title}\",\"{job.Url}\",\"{job.DateFound:dd/MM/yyyy}\",\"{job.Advertiser}\",";
}