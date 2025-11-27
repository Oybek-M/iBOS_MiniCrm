using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCrm.Domain.Enums;
using SmartCrm.Service.DTOs.Students;
using SmartCrm.Service.Interfaces.Students;

namespace SmartCrm.Controllers.Common
{
    [Route("api/students")]
    [ApiController]
    [Authorize(Roles = "SuperAdministrator, Administrator, Teacher")]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentsController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateStudentDto dto)
        {
            var newStudent = await _studentService.CreateAsync(dto);

            return CreatedAtRoute("GetStudentById", new { id = newStudent.Id }, newStudent);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var students = await _studentService.GetAllAsync();
            return Ok(students);
        }

        [HttpGet("filter-by-payment-status")]
        public async Task<IActionResult> GetByPaymentStatusAsync([FromQuery] MonthlyPaymentStatus status)
        {
            var result = await _studentService.GetStudentsByPaymentStatusAsync(status);
            return Ok(result);
        }

        [HttpGet("{id}", Name = "GetStudentById")] 
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            var student = await _studentService.GetByIdAsync(id);
            return Ok(student);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateStudentDto dto)
        {
            var updatedStudent = await _studentService.UpdateAsync(id, dto);
            return Ok(updatedStudent);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            await _studentService.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchStudentsAsync([FromQuery] string searchTerm)
        {
            var students = await _studentService.SearchAsync(searchTerm);
            return Ok(students);
        }
    }
}