using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCrm.Service.DTOs.Groups;
using SmartCrm.Service.Interfaces.Groups;
using System;
using System.Threading.Tasks;

namespace SmartCrm.Controllers.Common
{
    [Route("api/groups")]
    [ApiController]
    [Authorize(Roles = "SuperAdministrator, Administrator, Teacher")]
    public class GroupsController : ControllerBase
    {
        private readonly IGroupService _groupService;

        public GroupsController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateGroupDto dto)
        {
            var newGroup = await _groupService.CreateAsync(dto);
            return CreatedAtRoute("GetGroupById", new { id = newGroup.Id }, newGroup);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var groups = await _groupService.GetAllAsync();
            return Ok(groups);
        }

        [HttpGet("{id}", Name = "GetGroupById")]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            var group = await _groupService.GetByIdAsync(id);
            return Ok(group);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateGroupDto dto)
        {
            var updatedGroup = await _groupService.UpdateAsync(id, dto);
            return Ok(updatedGroup);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            await _groupService.DeleteAsync(id);
            return NoContent();
        }
    }
}