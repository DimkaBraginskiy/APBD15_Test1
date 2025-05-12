using Microsoft.Data.SqlClient;

namespace APBD15_Test1.Repositories;

public class ServicesRepository : IServicesRepository
{
    
    private readonly string _connectionString;

    public ServicesRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }
    
    public async Task<bool> ServiceExistsByNameAsync(CancellationToken token, string name)
    {
        string sql = @"SELECT 1 FROM Service WHERE name = @Name";

        await using (var connection = new SqlConnection(_connectionString))
        await using (var command = new SqlCommand(sql, connection))
        {
            await connection.OpenAsync(token);

            command.Parameters.AddWithValue("@Name", name);

            var result = command.ExecuteReaderAsync(token);

            return result != null;
        }   
    }
}