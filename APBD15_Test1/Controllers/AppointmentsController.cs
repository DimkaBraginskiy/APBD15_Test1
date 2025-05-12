using apbd_testPractice1.DTOs;
using apbd_testPractice1.Exceptions;
using APBD15_Test1.Services;
using Microsoft.AspNetCore.Mvc;

namespace APBD15_Test1.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AppointmentsController : ControllerBase
{
    private readonly IAppointmentsService _appointmentsService;

    public AppointmentsController(IAppointmentsService appointmentsService)
    {
        _appointmentsService = appointmentsService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAppointmentByIdAsync(CancellationToken token, int id)
    {
        var appointment = await _appointmentsService.GetAppointmentByIdAsync(token, id);
        if (appointment == null)
        {
            return NotFound($"Appointment with {id} was not found.");
        }

        return Ok(appointment);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAppointmentsAsync(
        CancellationToken token, [FromBody] AppointmentRequestDto dto)
    {
        try
        {
            var id = await _appointmentsService.CreateAppointmentAsync(token, dto);

            return Ok(new { Id = id });
        }
        catch (ConflictException ex)
        {
            return Conflict(new { Message = ex.Message });
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
        catch (BadRequestException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Error during data validation occurred:{ex.Message}" });
        }
    }
    
    


}

