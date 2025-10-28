using Server.Dtos;

namespace Server.Services;

public interface IFhirService
{
    Task<object> CreatePatientFromFhirAsync(HttpRequest request, CancellationToken token);

    object DeletePatient(long id, CancellationToken token);

    object GetAllPatientsAsFhirBundle(CancellationToken token);

    IEnumerable<FhirStoredMessageDto> GetMessages(CancellationToken token);
}