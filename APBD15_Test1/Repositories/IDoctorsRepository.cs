using APBD15_Test1.DTOs;

namespace APBD15_Test1.Repositories;

public interface IDoctorsRepository
{
    public Task<bool> ExistsAppointmentAsync(CancellationToken token, int id);
    public Task<DoctorResponseDto> GetDoctorByPwzAsync(CancellationToken token, string pwz);
}