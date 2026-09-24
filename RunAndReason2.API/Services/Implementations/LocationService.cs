using RunAndReason2.API.Exceptions;
using RunAndReason2.API.Models.DTOs;
using RunAndReason2.API.Repositories.Interfaces;
using RunAndReason2.API.Services.Interfaces;

namespace RunAndReason2.API.Services.Implementations;

public class LocationService : ILocationService
{
    private readonly ILocationRepository _locationRepository;

    public LocationService(ILocationRepository locationRepository)
    {
        _locationRepository = locationRepository;
    }

    public async Task<IEnumerable<LocationDto>> GetAllLocationsAsync(CancellationToken cancellationToken = default)
    {
        var locations = await _locationRepository.GetAllAsync(cancellationToken);
        return locations.Select(l => l.ToDto());
    }

    public async Task<LocationDto?> GetLocationByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var location = await _locationRepository.GetByIdAsync(id, cancellationToken);
        return location?.ToDto();
    }

    public async Task<LocationDto> CreateLocationAsync(CreateLocationDto dto, CancellationToken cancellationToken = default)
    {
        var entity = dto.ToEntity();
        var created = await _locationRepository.AddAsync(entity, cancellationToken);
        return created.ToDto();
    }

    public async Task UpdateLocationAsync(int id, UpdateLocationDto dto, CancellationToken cancellationToken = default)
    {
        var existing = await _locationRepository.GetByIdAsync(id, cancellationToken);
        if (existing == null)
        {
            throw new NotFoundException("Location", id);
        }

        dto.ApplyTo(existing);
        await _locationRepository.UpdateAsync(existing, cancellationToken);
    }

    public async Task DeleteLocationAsync(int id, CancellationToken cancellationToken = default)
    {
        var exists = await _locationRepository.ExistsAsync(id, cancellationToken);
        if (!exists)
        {
            throw new NotFoundException("Location", id);
        }

        await _locationRepository.DeleteAsync(id, cancellationToken);
    }
}