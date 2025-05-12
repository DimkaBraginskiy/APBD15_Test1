using System.ComponentModel.DataAnnotations;
using APBD15_Test1.DTOs;

namespace apbd_testPractice1.DTOs;

public class AppointmentRequestDto
{
    [Required, Range(1, Int32.MaxValue)]
    public int AppointmentId { get; set; }
    [Required, Range(1, Int32.MaxValue)]
    public int PatientId { get; set; }
    [Required, MinLength(7)]
    public string Pwz { get; set; }

    [Required]
    public List<ServiceResponseDto> Services { get; set; }
}