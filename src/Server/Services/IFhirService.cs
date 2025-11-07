using Server.Dtos;
using Server.Models;

namespace Server.Services;

public interface IFhirService
{
    Task<object> CreatePatientFromFhirAsync(HttpRequest request, CancellationToken token);

    Task<object> DeletePatient(long id, CancellationToken token);

    Task<object> GetAllPatientsAsFhirBundle(CancellationToken token);

    Task<IEnumerable<FhirLog>> GetMessages(CancellationToken token);
}