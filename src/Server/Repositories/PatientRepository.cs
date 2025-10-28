using Server.Models;

namespace Server.Repositories;

public class PatientRepository : IPatientRepository
{
    private readonly List<Patient> _patients = new();
    private long _nextId = 1;

    public long CreatePatient(Patient patient, CancellationToken token)
    {
        patient.Id = _nextId++;
        _patients.Add(patient);
        return patient.Id;
    }

    public Patient? GetPatient(long id, CancellationToken token)
    {
        return _patients.FirstOrDefault(p => p.Id == id);
    }

    public IEnumerable<Patient> GetPatients(CancellationToken token)
    {
        return _patients.ToList();
    }

    public bool DeletePatient(long id, CancellationToken token)
    {
        var patient = _patients.FirstOrDefault(p => p.Id == id);
        if (patient == null)
            return false;

        _patients.Remove(patient);
        return true;
    }
}