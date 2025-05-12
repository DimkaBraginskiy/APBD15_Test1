using APBD15_Test1.DTOs;
using Microsoft.Data.SqlClient;

namespace APBD15_Test1.Repositories;

public class DoctorsRepository : IDoctorsRepository
{
    private readonly string _connectionString;

    public DoctorsRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task<bool> ExistsAppointmentAsync(CancellationToken token, int id)
    {
        string sql = @"select * from Doctor WHERE doctor_id = @Id";

        await using (var connection = new SqlConnection(_connectionString))
        await using (var command = new SqlCommand(sql, connection))
        {
            await connection.OpenAsync(token);

            command.Parameters.AddWithValue("@Id", id);

            var result = command.ExecuteReaderAsync(token);

            return result != null;
        }
    }

    public async Task<DoctorResponseDto> GetDoctorByPwzAsync(CancellationToken token, string pwz)
    {
        string sql = @"select doctor_id, pwz FROM Doctor 
                        WHERE pwz = @pwz";
        
        DoctorResponseDto? doctor = null;
        
        await using (var connection = new SqlConnection(_connectionString))
        await using (var command = new SqlCommand(sql, connection))
        {
            await connection.OpenAsync(token);

            command.Parameters.AddWithValue("@pwz", pwz);

            var reader = await command.ExecuteReaderAsync(token);

            if (await reader.ReadAsync(token))
            {
                if (doctor == null)
                {
                    doctor = new DoctorResponseDto()
                    {
                        DoctorId = reader.GetInt32(reader.GetOrdinal("doctor_id")),
                        Pwz = reader.GetString(reader.GetOrdinal("PWZ"))
                    };    
                }
            }
        }

        return doctor;
    }



}