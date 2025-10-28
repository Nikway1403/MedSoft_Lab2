using Server.Models;

namespace Server.Repositories;

public interface IPatientRepository
{
    long CreatePatient(Patient patient, CancellationToken token);
    
    Patient GetPatient(long id, CancellationToken token);
    
    IEnumerable<Patient> GetPatients(CancellationToken token);
    
    bool DeletePatient(long id, CancellationToken token);
}