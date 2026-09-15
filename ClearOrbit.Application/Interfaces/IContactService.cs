using ClearOrbit.Application.Common;
using ClearOrbit.Application.DTOs.Contacts;

namespace ClearOrbit.Application.Interfaces;

public interface IContactService
{
    Task<Result<ContactResponseDto>> CreateAsync(CreateContactDto request);
    Task<Result<ContactResponseDto>> GetByIdAsync(Guid id);
    Task<Result<List<ContactResponseDto>>> GetByClientAsync(Guid clientId);
    Task<Result<List<ContactResponseDto>>> GetAllAsync();
    Task<Result<ContactResponseDto>> UpdateAsync(Guid id, UpdateContactDto request);
    Task<Result<bool>> DeleteAsync(Guid id);
}