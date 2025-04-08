using JobScraper.Console.Data;
using JobScraper.Console.Model;

var dataFolder = $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}/JobSearch";

using var db = new AppDbContext(dataFolder);
// Create the database and schema if they don't exist
db.Database.EnsureCreated();

var command = args[0].ToLower().Trim();

if (command == "help")
{
    HandleHelpCommand();
    return;
}

if (command == "import-jobs")
{
    await HandleImportJobsCommand();
    return;
}

if (command == "export-jobs")
{
    await HandleExportJobsCommand(args);
    return;
}

if (command == "exclude-company")
{
    await HandleExcludeCompanyCommand(args);
    return;
}

if (command == "update-status")
{
    await HandleMarkJobsCommand(args);
    return;
}

HandleHelpCommand();






async Task HandleMarkJobsCommand(string[] args)
{
    var jobIds = args[1].Split(',', StringSplitOptions.RemoveEmptyEntries)
        .Select(x => Convert.ToInt32(x)).ToList();
    var status = (JobStatus)Enum.Parse(typeof(JobStatus), args[2], true);
    // ids, [Not Interested, Interested, Applied, CompanyIsADumbDumb, New]
    
    await new JobStatusService(db).UpdateJobStatus(jobIds, status);
}

async Task HandleExcludeCompanyCommand(string[] args)
{
    var advertiser = args[1];
    var company = args[2];
    
    await new ExclusionsService(db).AddExclusion(advertiser, company);
}

async Task HandleExportJobsCommand(string[] args)
{
    var exporter = new JobExporter(db);
    var exportPath = $"{dataFolder}/Exports";

    var exportAll = args.Length == 1;

    if (exportAll)
    {
        await exporter.ExportAll(exportPath);
        return;
    }

    var status = (JobStatus)Enum.Parse(typeof(JobStatus), args[1], true);

    await new JobExporter(db).ExportForStatus(exportPath, status);
}

async Task HandleImportJobsCommand()
{
    await new JobImporter(db).Import($"{dataFolder}/Imports");
}

void HandleHelpCommand()
{
    Console.WriteLine();
    Console.WriteLine("import-jobs - Imports jobs that you have created using your script");

    Console.WriteLine();
    Console.WriteLine();
    Console.WriteLine();

    Console.WriteLine("export-jobs - Exports all jobs to a csv file");
    Console.WriteLine();
    Console.WriteLine("export-jobs [Status New|NotInterested|Interested|Applied|Offer|OfferAccepted|CompanyIsADumbDumb] - Exports with given status to a csv file");
    Console.WriteLine("e.g. export-jobs New");

    Console.WriteLine();
    Console.WriteLine();
    Console.WriteLine();

    Console.WriteLine("exclude-company [advertiser] [company] - excludes a company(probably a recruiter) from search results for the given advertiser");
    Console.WriteLine("e.g. exclude-company Indeed \"Hellish Recruiters\"");

    Console.WriteLine();
    Console.WriteLine();
    Console.WriteLine();

    Console.WriteLine("update-status [comma separated list of job ids(no spaces)] [job status]");
    Console.WriteLine("update-status 1,2,15,7 NotInterested");
    Console.WriteLine();
}