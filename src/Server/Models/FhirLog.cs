namespace Server.Models;

public class FhirLog
{
    public Guid Id { get; set; }
    public string RawJson { get; set; }

    public DateTime ReceivedAtUtc { get; set; }
}