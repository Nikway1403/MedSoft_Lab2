using Microsoft.EntityFrameworkCore;
using Server.Database;
using Server.Models;

namespace Server.Repositories;

public class FhirMessageRepository : IFhirMessageRepository
{
    private readonly ApplicationDbContext _context;

    public FhirMessageRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddMessage(string rawJson, DateTime receivedAtUtc, CancellationToken token)
    {
        await _context.FhirLogs.AddAsync(new FhirLog
        {
            RawJson = rawJson,
            ReceivedAtUtc = receivedAtUtc
        }, token);
        
        await _context.SaveChangesAsync(token);
    }
    
    public async Task<IEnumerable<FhirLog>> GetMessages(CancellationToken token)
    {
        return await _context.FhirLogs.ToListAsync(token);
    }
}