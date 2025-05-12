using apbd_testPractice1.DTOs;
using apbd_testPractice1.Exceptions;
using APBD15_Test1.DTOs;
using Microsoft.Data.SqlClient;

namespace APBD15_Test1.Repositories;

public class AppointmentsRepository : IAppointmentsRepository
{
    private readonly string _connectionString;

    public AppointmentsRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task<bool> ExistsAppointmentAsync(CancellationToken token, int id)
    {
        string sql = @"SELECT 1 FROM Appointment WHERE appoitment_id = @AppointmentId";

        await using (var connection = new SqlConnection(_connectionString))
        await using (var command = new SqlCommand(sql, connection))
        {
            await connection.OpenAsync(token);

            command.Parameters.AddWithValue("@AppointmentId", id);

            var result = command.ExecuteReaderAsync(token);

            return result != null;
        }
    }


    public async Task<AppointmentResponseDto> GetAppointmentByIdAsync(CancellationToken token, int id)
    {
        string sql = @"SELECT
            a.date,
         a.patient_id,
         a.doctor_id,
         d.Pwz,
           p.first_name,
           p.last_name,
           p.date_of_birth,
        s.name,
           aser.service_fee

        FROM Appointment a
         JOIN Doctor d ON a.doctor_id = d.doctor_id
         JOIN Patient p ON a.patient_id = p.patient_id
         JOIN Appointment_Service aser ON a.appoitment_id = aser.appoitment_id
         JOIN Service s ON aser.service_id = s.service_id
        WHERE a.appoitment_id = @Id
        ORDER BY a.doctor_id";


        AppointmentResponseDto? appointment = null;

        await using (var connection = new SqlConnection(_connectionString))
        await using (var command = new SqlCommand(sql, connection))
        {
            await connection.OpenAsync(token);

            command.Parameters.AddWithValue("@Id", id);

            var reader = await command.ExecuteReaderAsync(token);

            while (await reader.ReadAsync(token))
            {
                if (appointment == null)
                {
                    appointment = new AppointmentResponseDto()
                    {
                        Date = reader.GetDateTime(reader.GetOrdinal("date")),
                        Patient = new PatientResponseDto()
                        {
                            FirstName = reader.GetString(reader.GetOrdinal("first_name")),
                            LastName = reader.GetString(reader.GetOrdinal("last_name")),
                            DateOfBirth = reader.GetDateTime(reader.GetOrdinal("date_of_birth"))
                        },
                        Doctor = new DoctorResponseDto()
                        {
                            DoctorId = reader.GetInt32(reader.GetOrdinal("doctor_id")),
                            Pwz = reader.GetString(reader.GetOrdinal("PWZ"))
                        },
                        Services = new List<ServiceResponseDto>()
                    };
                }

                appointment.Services.Add(
                    new ServiceResponseDto()
                    {
                        Name = reader.GetString(reader.GetOrdinal("name")),
                        ServiceFee = reader.GetDecimal(reader.GetOrdinal("service_fee"))
                    });
            }
        }

        return appointment;
    }
    
     public async Task<int> CreateAppointmentAsync(CancellationToken token, AppointmentRequestDto appointment, int doctorId)
    {
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync(token);

        var transaction = await conn.BeginTransactionAsync(token);

        try
        {
            // inserting into appointment:
            const string insertAppointmentSql = @"
            INSERT INTO appointment (appoitment_id, patient_id, doctor_id, date)
            VALUES (@appointment_id, @patient_id, @doctor_id, @date)";

            using var insertAppointmentCmd = new SqlCommand(insertAppointmentSql, conn, (SqlTransaction)transaction);
            insertAppointmentCmd.Parameters.AddWithValue("@appointment_id", appointment.AppointmentId);
            insertAppointmentCmd.Parameters.AddWithValue("@patient_id", appointment.PatientId);
            insertAppointmentCmd.Parameters.AddWithValue("@doctor_id", doctorId);
            insertAppointmentCmd.Parameters.AddWithValue("@date", DateTime.Now);
            await insertAppointmentCmd.ExecuteNonQueryAsync(token);

            
            foreach (var service in appointment.Services)
            {
                int serviceId;

                const string getServiceIdSql = @"
                    SELECT TOP 1 service_id 
                    FROM service 
                    WHERE name = @name
                    ORDER BY service_id DESC;";

                using var getServiceIdCmd = new SqlCommand(getServiceIdSql, conn, (SqlTransaction)transaction);
                getServiceIdCmd.Parameters.AddWithValue("@name", service.Name);
                var serviceIdObj = await getServiceIdCmd.ExecuteScalarAsync(token);
                if (serviceIdObj is null)
                    throw new NotFoundException($"Service '{service.Name}' not found.");
                
                serviceId = Convert.ToInt32(serviceIdObj);

                const string insertAppointmentServiceSql = @"
                    INSERT INTO appointment_service (appointment_id, service_id, service_fee)
                    VALUES (@appointment_id, @service_id, @service_fee);";

                using var insertServiceCmd = new SqlCommand(insertAppointmentServiceSql, conn, (SqlTransaction)transaction);
                insertServiceCmd.Parameters.AddWithValue("@appointment_id", appointment.AppointmentId);
                insertServiceCmd.Parameters.AddWithValue("@service_id", serviceId);
                insertServiceCmd.Parameters.AddWithValue("@service_fee", service.ServiceFee);
                await insertServiceCmd.ExecuteNonQueryAsync(token);
            }

            await transaction.CommitAsync(token);
            return appointment.AppointmentId;
        }
        catch
        {
            await transaction.RollbackAsync(token);
            throw;
        }
    }
}