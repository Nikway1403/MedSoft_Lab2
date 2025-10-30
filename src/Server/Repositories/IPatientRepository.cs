using Server.Models;

namespace Server.Repositories;

public interface IPatientRepository
{
    Task<long> CreatePatient(Patient patient, CancellationToken token);
    
    Task<Patient?> GetPatient(long id, CancellationToken token);
    
    Task<IEnumerable<Patient>> GetPatients(CancellationToken token);
    
    Task<bool>  DeletePatient(long id, CancellationToken token);
}