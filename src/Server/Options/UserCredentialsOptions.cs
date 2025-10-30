using Server.Models;

namespace Server.Options;

public class UserCredentialsOptions
{
    public List<UserRecord> Users { get; set; } = new();
}