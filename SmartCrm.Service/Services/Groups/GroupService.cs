using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SmartCrm.Data.Interfaces;
using SmartCrm.Domain.Entities.Groups;
using SmartCrm.Service.Common.Exceptions;
using SmartCrm.Service.DTOs.Groups;
using SmartCrm.Service.Interfaces.Groups;
using System.Net;

namespace SmartCrm.Service.Services.Groups
{
    public class GroupService : IGroupService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateGroupDto> _createValidator;

        public GroupService(IUnitOfWork unitOfWork, IMapper mapper, IValidator<CreateGroupDto> createValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _createValidator = createValidator;
        }

        public async Task<GroupDto> CreateAsync(CreateGroupDto dto)
        {
            var validationResult = await _createValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
                throw new ValidatorException(string.Join("\n", validationResult.Errors));

            var teacher = await _unitOfWork.Teacher.GetById(dto.TeacherId);
            if (teacher is null)
                throw new StatusCodeException(HttpStatusCode.NotFound, "Bunday IDga ega o'qituvchi topilmadi.");

            var existingGroup = await _unitOfWork.Group.FirstOrDefaultAsync(g => g.Name.ToLower() == dto.Name.ToLower());
            if (existingGroup is not null)
                throw new StatusCodeException(HttpStatusCode.Conflict, "Bu nomdagi guruh allaqachon mavjud.");

            var newGroup = new Group { Name = dto.Name, TeacherId = dto.TeacherId };
            await _unitOfWork.Group.Add(newGroup);

            return _mapper.Map<GroupDto>(newGroup);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var group = await _unitOfWork.Group.GetById(id);
            if (group is null)
                throw new StatusCodeException(HttpStatusCode.NotFound, "Guruh topilmadi.");

            bool hasStudents = await _unitOfWork.Student.GetAll().AnyAsync(s => s.GroupId == id);
            if (hasStudents)
                throw new StatusCodeException(HttpStatusCode.BadRequest, "Guruhni o'chirib bo'lmaydi, chunki unda o'quvchilar mavjud.");

            await _unitOfWork.Group.Remove(group);
            return true;
        }

        public async Task<IEnumerable<GroupDto>> GetAllAsync()
        {
            var groups = await _unitOfWork.Group.GetAll()
                .Include(g => g.Teacher)
                .Include(g => g.Students)
                .ToListAsync();

            return _mapper.Map<IEnumerable<GroupDto>>(groups);
        }

        public async Task<GroupDto> GetByIdAsync(Guid id)
        {
            var group = await _unitOfWork.Group.GetAll()
                .Include(g => g.Teacher)
                .Include(g => g.Students)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (group is null)
                throw new StatusCodeException(HttpStatusCode.NotFound, "Guruh topilmadi.");

            return _mapper.Map<GroupDto>(group);
        }

        public async Task<GroupDto> UpdateAsync(Guid id, UpdateGroupDto dto)
        {
            var group = await _unitOfWork.Group.GetById(id);
            if (group is null)
                throw new StatusCodeException(HttpStatusCode.NotFound, "Guruh topilmadi.");

            var existingGroup = await _unitOfWork.Group.FirstOrDefaultAsync(g => g.Name.ToLower() == dto.Name.ToLower() && g.Id != id);
            if (existingGroup is not null)
                throw new StatusCodeException(HttpStatusCode.Conflict, "Bu nomdagi guruh allaqachon mavjud.");

            var teacher = await _unitOfWork.Teacher.GetById(dto.TeacherId);
            if (teacher is null)
                throw new StatusCodeException(HttpStatusCode.NotFound, "Bunday IDga ega o'qituvchi topilmadi.");

            group.Name = dto.Name;
            group.TeacherId = dto.TeacherId;

            await _unitOfWork.Group.Update(group);

            var updatedGroup = await GetByIdAsync(id);
            return updatedGroup;
        }
    }
}