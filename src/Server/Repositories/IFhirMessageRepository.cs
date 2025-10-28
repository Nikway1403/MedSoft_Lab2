using Server.Dtos;

namespace Server.Repositories;

public interface IFhirMessageRepository
{
    void AddMessage(string rawJson, DateTime receivedAtUtc);

    IEnumerable<FhirStoredMessageDto> GetMessages(CancellationToken token);
}