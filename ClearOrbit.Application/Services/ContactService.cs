using ClearOrbit.Application.Common;
using ClearOrbit.Application.DTOs.Contacts;
using ClearOrbit.Application.Guards;
using ClearOrbit.Application.Interfaces;
using ClearOrbit.Domain.Entities;
using ClearOrbit.Domain.ValueObjects;

namespace ClearOrbit.Application.Services;

public class ContactService : IContactService
{
    private readonly IContactRepository _contactRepo;
    private readonly IClientRepository _clientRepo;

    public ContactService(IContactRepository contactRepo, IClientRepository clientRepo)
    {
        _contactRepo = contactRepo;
        _clientRepo = clientRepo;
    }

    public async Task<Result<ContactResponseDto>> CreateAsync(CreateContactDto request)
    {
        var errors = ContactGuard.Validate(request);
        if (errors.Any()) return Result<ContactResponseDto>.Fail(errors);

        var client = await _clientRepo.GetByIdAsync(request.ClientId);
        if (client is null) return Result<ContactResponseDto>.Fail("Client not found.");

        if (await _contactRepo.ExistsWithEmailInClientAsync(request.Email, request.ClientId))
            return Result<ContactResponseDto>.Fail(
                "A contact with that email already exists for this client.");

        var contact = new Contact
        {
            ClientId = request.ClientId,
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = new EmailAddress(request.Email),
            Phone = request.Phone?.Trim(),
            JobTitle = request.JobTitle?.Trim(),
            Role = request.Role
        };

        await _contactRepo.AddAsync(contact);
        await _contactRepo.SaveChangesAsync();

        return Result<ContactResponseDto>.Ok(
            MapToDto(contact, client),
            $"{contact.FirstName} added as a contact.");
    }

    public async Task<Result<ContactResponseDto>> GetByIdAsync(Guid id)
    {
        var contact = await _contactRepo.GetByIdAsync(id);
        if (contact is null) return Result<ContactResponseDto>.Fail("Contact not found.");
        return Result<ContactResponseDto>.Ok(MapToDto(contact, contact.Client));
    }

    public async Task<Result<List<ContactResponseDto>>> GetByClientAsync(Guid clientId)
    {
        var contacts = await _contactRepo.GetByClientAsync(clientId);
        return Result<List<ContactResponseDto>>.Ok(
            contacts.Select(c => MapToDto(c, c.Client)).ToList());
    }

    public async Task<Result<List<ContactResponseDto>>> GetAllAsync()
    {
        var contacts = await _contactRepo.GetAllAsync();
        return Result<List<ContactResponseDto>>.Ok(
            contacts.Select(c => MapToDto(c, c.Client)).ToList());
    }

    public async Task<Result<ContactResponseDto>> UpdateAsync(Guid id, UpdateContactDto request)
    {
        var errors = ContactGuard.Validate(request);
        if (errors.Any()) return Result<ContactResponseDto>.Fail(errors);

        var contact = await _contactRepo.GetByIdAsync(id);
        if (contact is null) return Result<ContactResponseDto>.Fail("Contact not found.");

        if (await _contactRepo.ExistsWithEmailInClientAsync(request.Email, contact.ClientId, id))
            return Result<ContactResponseDto>.Fail(
                "Another contact for this client already uses that email.");

        contact.FirstName = request.FirstName.Trim();
        contact.LastName = request.LastName.Trim();
        contact.Email = new EmailAddress(request.Email);
        contact.Phone = request.Phone?.Trim();
        contact.JobTitle = request.JobTitle?.Trim();
        contact.Role = request.Role;

        await _contactRepo.SaveChangesAsync();
        return Result<ContactResponseDto>.Ok(MapToDto(contact, contact.Client), "Contact updated.");
    }

    public async Task<Result<bool>> DeleteAsync(Guid id)
    {
        var contact = await _contactRepo.GetByIdAsync(id);
        if (contact is null) return Result<bool>.Fail("Contact not found.");

        contact.IsActive = false;
        await _contactRepo.SaveChangesAsync();
        return Result<bool>.Ok(true, "Contact removed.");
    }

    private static ContactResponseDto MapToDto(Contact c, Client? client) => new()
    {
        Id = c.Id,
        ClientId = c.ClientId,
        ClientName = client?.Name ?? string.Empty,
        FirstName = c.FirstName,
        LastName = c.LastName,
        FullName = $"{c.FirstName} {c.LastName}".Trim(),
        Email = c.Email.Value,
        Phone = c.Phone,
        JobTitle = c.JobTitle,
        Role = (int)c.Role,
        RoleName = c.Role.ToString(),
        CreatedAt = c.CreatedAt
    };
}