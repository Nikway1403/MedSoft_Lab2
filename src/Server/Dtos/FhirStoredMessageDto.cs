namespace Server.Dtos;

public class FhirStoredMessageDto
{
    public string RawJson { get; set; } = string.Empty;
    
    public DateTime ReceivedAtUtc { get; set; }
}
