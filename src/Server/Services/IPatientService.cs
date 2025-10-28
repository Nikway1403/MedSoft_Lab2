using Server.Dtos;

namespace Server.Services;

public interface IPatientService
{
    PatientDto CreatePatient(PatientDto patient, CancellationToken token);

    PatientDto? GetPatient(long id, CancellationToken token);

    IEnumerable<PatientDto> GetAllPatients(CancellationToken token);

    bool DeletePatient(long id, CancellationToken token);
}