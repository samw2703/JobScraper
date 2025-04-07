namespace JobScraper.Console.Model;

public class Job
{
    public int Id { get; set; }
    public string Advertiser { get; set; }
    public string? AdvertiserId { get; set; }
    public string Title { get; set; }
    public string Company { get; set; }
    public string Url { get; set; }
    public DateTime DateFound { get; set; }
    public JobStatus Status { get; set; }
}

public enum JobStatus
{
    New,
    NotInterested,
    Interested,
    Applied,
    Offer,
    OfferAccepted,
    CompanyIsADumbDumb
}
