using Microsoft.Data.SqlClient;

namespace APBD15_Test1.Repositories;

public class PatientsesRepository : IPatientsRepository
{
    
    private readonly string _connectionString;

    public PatientsesRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }
    
    public async Task<bool> ExistsPatientAsync(CancellationToken token, int id)
    {
        string sql = @"select * from Patient WHERE patient_id = @Id";

        await using (var connection = new SqlConnection(_connectionString))
        await using (var command = new SqlCommand(sql, connection))
        {
            await connection.OpenAsync(token);

            command.Parameters.AddWithValue("@Id", id);

            var result = command.ExecuteReaderAsync(token);

            return result != null;
        }
    }
}