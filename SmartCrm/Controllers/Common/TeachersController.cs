using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCrm.Service.DTOs.Teachers;
using SmartCrm.Service.Interfaces.Teachers;
using System;
using System.Threading.Tasks;

namespace SmartCrm.Controllers.Common
{
    [Route("api/teachers")]
    [ApiController]
    [Authorize(Roles = "SuperAdministrator, Administrator, Teacher")]
    public class TeachersController : ControllerBase
    {
        private readonly ITeacherService _teacherService;

        public TeachersController(ITeacherService teacherService)
        {
            _teacherService = teacherService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateTeacherDto dto)
        {
            var newTeacher = await _teacherService.CreateAsync(dto);

            return CreatedAtRoute("GetTeacherById", new { id = newTeacher.Id }, newTeacher);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var teachers = await _teacherService.GetAllAsync();
            return Ok(teachers);
        }

        [HttpGet("{id}", Name = "GetTeacherById")] 
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            var teacher = await _teacherService.GetByIdAsync(id);
            return Ok(teacher);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateTeacherDto dto)
        {
            var updatedTeacher = await _teacherService.UpdateAsync(id, dto);
            return Ok(updatedTeacher);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            await _teacherService.DeleteAsync(id);
            return NoContent();
        }
    }
}