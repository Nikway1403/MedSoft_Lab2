using Microsoft.EntityFrameworkCore;
using Server.Database;
using Server.Models;

namespace Server.Repositories;

public class PatientRepository : IPatientRepository
{
    private readonly ApplicationDbContext _dbContext;

    public PatientRepository(ApplicationDbContext  dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<long> CreatePatient(Patient patient, CancellationToken token)
    {
        var res = await _dbContext.Patients.AddAsync(patient, token);
        await _dbContext.SaveChangesAsync(token);
        return res.Entity.Id;
    }

    public async Task<Patient?> GetPatient(long id, CancellationToken token)
    {
        var patient = await _dbContext.Patients.FirstOrDefaultAsync(p => p.Id == id, token);
        return patient;
    }

    public async Task<IEnumerable<Patient>> GetPatients(CancellationToken token)
    {
        return await _dbContext.Patients.ToListAsync(token);
    }

    public async Task<bool> DeletePatient(long id, CancellationToken token)
    {
        var patient = await _dbContext.Patients.FirstOrDefaultAsync(p => p.Id == id, token);
        if (patient == null)
            return false;

        _dbContext.Patients.Remove(patient);
        await _dbContext.SaveChangesAsync(token);
        return true;
    }
}