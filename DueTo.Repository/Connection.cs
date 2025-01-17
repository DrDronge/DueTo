using System.Data;
using Microsoft.Data.SqlClient;

namespace DueTo.Repository;

public class Connection
{
    private static SqlConnection? _connection;
    private static readonly Lock Lock = new();

    public static SqlConnection GetConnection()
    {
        //Checked twice, first here for efficiency. 
        if (_connection == null || _connection.State == ConnectionState.Closed ||
            _connection.State == ConnectionState.Broken)
        {
            lock (Lock)
            {
                //Checked here to combat race conditions, if two connections is trying to be created at the same time
                if (_connection == null || _connection.State == ConnectionState.Closed ||
                    _connection.State == ConnectionState.Broken)
                {
                    var connectionString = BuildConnectionString();
                    
                    _connection = new SqlConnection(connectionString);

                    try
                    {
                        _connection.Open();
                    }
                    catch (InvalidOperationException e)
                    {
                        throw new InvalidOperationException("Failed to establish a database connection.", e);
                    }
                }
            }
        }
        return _connection;
    }

    private static string BuildConnectionString()
    {
        var builder = new SqlConnectionStringBuilder
        {
            DataSource = Environment.GetEnvironmentVariable("DataSource"),
            UserID = Environment.GetEnvironmentVariable("UserID"),
            Password = Environment.GetEnvironmentVariable("Password"),
            InitialCatalog = Environment.GetEnvironmentVariable("InitialCatalog"),
        };
        
        return builder.ConnectionString;
    }

    public static void CloseConnection()
    {
        if (_connection != null && _connection.State != ConnectionState.Closed)
        {
            _connection.Close();
        }
    }
}