using Server.Dtos;

namespace Server.Repositories;

public class FhirMessageRepository : IFhirMessageRepository
{
    private readonly List<FhirStoredMessageDto> _messages = new();

    public void AddMessage(string rawJson, DateTime receivedAtUtc)
    {
        _messages.Add(new FhirStoredMessageDto
        {
            RawJson = rawJson,
            ReceivedAtUtc = receivedAtUtc
        });
    }
    
    public IEnumerable<FhirStoredMessageDto> GetMessages(CancellationToken token)
    {
        return _messages.ToList();
    }
}