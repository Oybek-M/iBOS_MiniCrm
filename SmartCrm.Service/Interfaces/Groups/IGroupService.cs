using SmartCrm.Service.DTOs.Groups;

namespace SmartCrm.Service.Interfaces.Groups;

public interface IGroupService
{
    Task<GroupDto> CreateAsync(CreateGroupDto dto);
    Task<GroupDto> GetByIdAsync(Guid id);
    Task<IEnumerable<GroupDto>> GetAllAsync();
    Task<GroupDto> UpdateAsync(Guid id, UpdateGroupDto dto);
    Task<bool> DeleteAsync(Guid id);
}
