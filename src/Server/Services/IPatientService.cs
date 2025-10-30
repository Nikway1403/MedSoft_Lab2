using Server.Dtos;

namespace Server.Services;

public interface IPatientService
{
    Task<PatientDto> CreatePatient(PatientDto patient, CancellationToken token);

    Task<PatientDto?> GetPatient(long id, CancellationToken token);

    Task<IEnumerable<PatientDto>> GetAllPatients(CancellationToken token);

    Task<bool> DeletePatient(long id, CancellationToken token);
}