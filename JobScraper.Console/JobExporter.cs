using JobScraper.Console.Data;
using JobScraper.Console.Model;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

public class JobExporter
{
    private readonly AppDbContext _dbContext;

    public JobExporter(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task ExportAll(string exportsFolder)
    {
        var jobs = await _dbContext.Jobs.ToListAsync();
        var csv = ToCsv(jobs);
        var filePath = CreateFilePath(exportsFolder, "all");

        File.WriteAllText(filePath, csv);

        Launch(filePath);
    } 

    public async Task ExportForStatus(string exportsFolder, JobStatus jobStatus)
    {
        var jobs = await _dbContext.Jobs.Where(x => x.Status == jobStatus).ToListAsync();
        var csv = ToCsv(jobs);
        var filePath = CreateFilePath(exportsFolder, jobStatus);

        File.WriteAllText(filePath, csv);

        Launch(filePath);
    }

    private void Launch(string filePath)
    {
        var psi = new ProcessStartInfo
        {
            FileName = Path.GetFullPath(filePath),
            UseShellExecute = true
        };

        Process.Start(psi);
    }

    private string CreateFilePath(string exportsFolder, JobStatus jobStatus)
        => CreateFilePath(exportsFolder, jobStatus.ToString().ToLower());

    private string CreateFilePath(string exportsFolder, string jobStatus)
    => $"{exportsFolder}/new{jobStatus}_{DateTime.Now:yyyy-MM-dd_HH-mm}.csv";

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
