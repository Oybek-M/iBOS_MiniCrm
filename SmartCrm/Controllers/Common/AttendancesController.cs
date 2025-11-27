using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCrm.Service.DTOs.Attendances;
using SmartCrm.Service.Interfaces.Attendances;
using System.Security.Policy;

namespace SmartCrm.Controllers.Common
{
    [Route("api/attendances")]
    [ApiController]
    [Authorize(Roles = "SuperAdministrator, Administrator, Teacher")]
    public class AttendancesController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;

        public AttendancesController(IAttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateAttendanceDto dto)
        {
            var newAttendance = await _attendanceService.CreateAsync(dto);
            return CreatedAtRoute("GetAttendanceById", new { id = newAttendance.Id }, newAttendance);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var attendances = await _attendanceService.GetAllAsync();
            return Ok(attendances);
        }

        [HttpGet("group/{groupId:guid}", Name = "GetAttendancesByGroupId")]
        public async Task<IActionResult> GetAttendancesByGroupIdAsync(Guid groupId)
        {
            var attendances = await _attendanceService.GetAttendancesByGroupIdAsync(groupId);
            return Ok(attendances);
        }

        [HttpGet("student/{studentId:guid}", Name = "GetAttendancesByStudentId")]
        public async Task<IActionResult> GetAttendancesByStudentIdAsync(Guid studentId)
        {
            var attendances = await _attendanceService.GetAttendancesByStudentIdAsync(studentId);
            return Ok(attendances);
        }

        [HttpGet("{id:guid}", Name = "GetAttendanceById")]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            var attendance = await _attendanceService.GetByIdAsync(id);
            if (attendance is null) return NotFound();
            return Ok(attendance);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateAttendanceDto dto)
        {
            var updatedAttendance = await _attendanceService.UpdateAsync(id, dto);
            return Ok(updatedAttendance);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            await _attendanceService.DeleteAsync(id);
            return NoContent();
        }
    }
}
