namespace JobScraper.Console.Model;

public class Area 
{
    public int Id { get; set; }
    public string Name { get; set; }

    public static Area Create(string name) => new Area { Name = name };
}
