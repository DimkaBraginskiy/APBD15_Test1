namespace APBD15_Test1.DTOs;

public class AppointmentResponseDto
{
    public DateTime Date { get; set; }
    public PatientResponseDto Patient { get; set; }
    public DoctorResponseDto Doctor { get; set; }
    public List<ServiceResponseDto> Services { get; set; }
}