using ClearOrbit.Application.Common;
using ClearOrbit.Application.DTOs.Clients;
using ClearOrbit.Application.Guards;
using ClearOrbit.Application.Interfaces;
using ClearOrbit.Domain.Entities;

namespace ClearOrbit.Application.Services;

public class ClientService : IClientService
{
    private readonly IClientRepository _clientRepository;
    private readonly IOrganizationRepository _organizationRepository;

    public ClientService(
        IClientRepository clientRepository,
        IOrganizationRepository organizationRepository)
    {
        _clientRepository = clientRepository;
        _organizationRepository = organizationRepository;
    }

    public async Task<Result<ClientResponseDto>> CreateAsync(CreateClientDto request)
    {
        var errors = ClientGuard.Validate(request);
        if (errors.Any()) return Result<ClientResponseDto>.Fail(errors);

        if (await _clientRepository.ExistsByNameAsync(request.Name))
            return Result<ClientResponseDto>.Fail("A client with that name already exists.");

        var org = await _organizationRepository.GetDefaultAsync();
        if (org is null)
            return Result<ClientResponseDto>.Fail("No organization configured. Contact admin.");

        var client = new Client
        {
            Name = request.Name.Trim(),
            Industry = request.Industry?.Trim(),
            Website = request.Website?.Trim(),
            Notes = request.Notes,
            OrganizationId = org.Id
        };

        await _clientRepository.AddAsync(client);
        await _clientRepository.SaveChangesAsync();

        return Result<ClientResponseDto>.Ok(MapToDto(client), "Client created successfully.");
    }

    public async Task<Result<ClientResponseDto>> GetByIdAsync(Guid id)
    {
        var client = await _clientRepository.GetByIdAsync(id);
        if (client is null) return Result<ClientResponseDto>.Fail("Client not found.");
        return Result<ClientResponseDto>.Ok(MapToDto(client));
    }

    public async Task<Result<List<ClientResponseDto>>> GetAllAsync()
    {
        var clients = await _clientRepository.GetAllAsync();
        return Result<List<ClientResponseDto>>.Ok(clients.Select(MapToDto).ToList());
    }

    public async Task<Result<ClientResponseDto>> UpdateAsync(Guid id, UpdateClientDto request)
    {
        var errors = ClientGuard.Validate(request);
        if (errors.Any()) return Result<ClientResponseDto>.Fail(errors);

        var client = await _clientRepository.GetByIdAsync(id);
        if (client is null) return Result<ClientResponseDto>.Fail("Client not found.");

        client.Name = request.Name.Trim();
        client.Industry = request.Industry?.Trim();
        client.Website = request.Website?.Trim();
        client.Notes = request.Notes;

        await _clientRepository.SaveChangesAsync();
        return Result<ClientResponseDto>.Ok(MapToDto(client), "Client updated successfully.");
    }

    public async Task<Result<bool>> DeleteAsync(Guid id)
    {
        var client = await _clientRepository.GetByIdAsync(id);
        if (client is null) return Result<bool>.Fail("Client not found.");

        // Soft delete
        client.IsActive = false;
        await _clientRepository.SaveChangesAsync();

        return Result<bool>.Ok(true, "Client deactivated.");
    }

    private static ClientResponseDto MapToDto(Client client) => new()
    {
        Id = client.Id,
        Name = client.Name,
        Industry = client.Industry,
        Website = client.Website,
        Notes = client.Notes,
        ContactCount = client.Contacts?.Count ?? 0,
        CreatedAt = client.CreatedAt
    };
}