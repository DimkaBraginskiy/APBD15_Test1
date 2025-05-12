using apbd_testPractice1.DTOs;
using apbd_testPractice1.Exceptions;
using APBD15_Test1.DTOs;
using APBD15_Test1.Repositories;

namespace APBD15_Test1.Services;

public class AppointmentsService : IAppointmentsService
{
   private readonly IAppointmentsRepository _appointmentsRepository;
   private readonly IPatientsRepository _patientsRepository;
   private readonly IDoctorsRepository _doctorsRepository;
   private readonly IServicesRepository _servicesRepository;

    public AppointmentsService (
        IAppointmentsRepository appointmentsRepository,
        IPatientsRepository patientsRepository,
        IDoctorsRepository doctorsRepository,
        IServicesRepository servicesRepository)
    {
        _appointmentsRepository = appointmentsRepository;
        _patientsRepository = patientsRepository;
        _doctorsRepository = doctorsRepository;
        _servicesRepository = servicesRepository;
    }
    
    public async Task<AppointmentResponseDto?> GetAppointmentByIdAsync(CancellationToken token, int id)
    {
        if (await _appointmentsRepository.ExistsAppointmentAsync(token, id) == false)
        {
            return null;
        }

        var appointment = await _appointmentsRepository.GetAppointmentByIdAsync(token, id);
        return appointment;
    }

    public async Task<int> CreateAppointmentAsync(CancellationToken token, AppointmentRequestDto dto)
    {
        //Validating all the passed data:
        if (await _appointmentsRepository.ExistsAppointmentAsync(token, dto.AppointmentId) == false)
        {
            throw new ConflictException($"Appointment with id {dto.AppointmentId} already exists.");
        }
        
        if (await _patientsRepository.ExistsPatientAsync(token, dto.PatientId) == false)
        {
            throw new ConflictException($"Patient with id {dto.PatientId} already exists.");
        }

        var doctor = await _doctorsRepository.GetDoctorByPwzAsync(token, dto.Pwz);
        if (doctor == null)
        {
            throw new NotFoundException($"Doctor with Pwz {dto.Pwz} not found.");
        }
        
        //going through services....
        foreach (var serviceRequestDto in dto.Services)
        {
            var service = await _servicesRepository.ServiceExistsByNameAsync(token, serviceRequestDto.Name);
            if (service == null)
            {
                throw new NotFoundException($"Service with Name {serviceRequestDto.Name} not found.");
            }
        }
        
        //!!!!!!!!!!!!!!!!!!!!!!!!!!! DOCTOR ID
        var result = await _appointmentsRepository.CreateAppointmentAsync(token, dto, doctor.DoctorId); 

        return result;
    }
}