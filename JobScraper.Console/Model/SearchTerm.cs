namespace JobScraper.Console.Model;

public class SearchTerm
{
    public int Id { get; set; }
    public string Value { get; set; }

    public static SearchTerm Create(string value) => new SearchTerm { Value = value };
}
