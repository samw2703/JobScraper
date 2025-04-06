using JobScraper.Console.Data;
using JobScraper.Console.Model;
using Microsoft.EntityFrameworkCore;

using var db = new AppDbContext();

// Create the database and schema if they don't exist
db.Database.EnsureCreated();

var areas = await db.Areas.ToListAsync();
var searchTerms = await db.SearchTerms.ToListAsync();

var command = args[0].ToLower().Trim();

if (command == "help")
{
    HandleHelpCommand();
    return;
}

if (command == "search-term")
{
    await HandleSearchTermCommand(args);
    return;
}

if (command == "area")
{
    await HandleAreaCommand(args);
    return;
}

void HandleHelpCommand()
{
    Console.WriteLine("search-term list");
    Console.WriteLine("search-term add <new search term>");

    Console.WriteLine();

    Console.WriteLine("area list");
    Console.WriteLine("area add <new area>");
}

async Task HandleAreaCommand(string[] args)
{
    var command = args[1].ToLower().Trim();
    if (command == "list")
    {
        HandleAreaListCommandAsync();
        return;
    }

    if (command == "add")
    {
        await HandleAreaAddCommand(args);
        return;
    }
}

async Task HandleAreaAddCommand(string[] args)
{
    var newArea = args[2].Trim();

    if (string.IsNullOrEmpty(newArea) || areas.Any(x => x.Name.ToLower() == newArea.ToLower()))
    {
        Console.WriteLine("cannot save this area");
        return;
    }

    await db.AddAsync(Area.Create(newArea));
    await db.SaveChangesAsync();
}

void HandleAreaListCommandAsync()
{
    foreach (var area in areas)
        Console.WriteLine(area.Name);
}

async Task HandleSearchTermCommand(string[] args)
{
    var command = args[1].ToLower().Trim();
    if (command == "list")
    {
        HandleSearchTermListCommand();
        return;
    }

    if (command == "add")
    {
        await HandleSearchTermAddCommandAsync(args);
        return;
    }
}

async Task HandleSearchTermAddCommandAsync(string[] args)
{
    var newSearchTerm = args[2].Trim();

    if (string.IsNullOrEmpty(newSearchTerm) || searchTerms.Any(x => x.Value.ToLower() == newSearchTerm.ToLower()))
    {
        Console.WriteLine("cannot save this search term");
        return;
    }

    await db.AddAsync(SearchTerm.Create(newSearchTerm));
    await db.SaveChangesAsync();
}

void HandleSearchTermListCommand()
{
    foreach (var searchTerm in searchTerms)
        Console.WriteLine(searchTerm.Value);
}