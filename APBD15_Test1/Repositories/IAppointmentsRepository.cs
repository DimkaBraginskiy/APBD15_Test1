using APBD15_Test1.DTOs;

namespace APBD15_Test1.Repositories;

public interface IAppointmentsRepository
{
    public Task<bool> ExistsAppointmentAsync(CancellationToken token, int id);
    
    public Task<AppointmentResponseDto> GetAppointmentByIdAsync(CancellationToken token, int id);
}