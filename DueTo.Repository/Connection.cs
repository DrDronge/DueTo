namespace DueTo.Repository;

public class Connection
{
    private readonly string _connectionString = Environment.GetEnvironmentVariable("DATABASE_URL");
}