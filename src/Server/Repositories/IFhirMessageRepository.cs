using Server.Models;

namespace Server.Repositories;

public interface IFhirMessageRepository
{
    Task AddMessage(string rawJson, DateTime receivedAtUtc , CancellationToken token);

    Task<IEnumerable<FhirLog>> GetMessages(CancellationToken token);
}