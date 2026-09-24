using RunAndReason2.API.Models.DTOs;

namespace RunAndReason2.API.Services.Interfaces;

public interface ILocationService
{
    Task<IEnumerable<LocationDto>> GetAllLocationsAsync(CancellationToken cancellationToken = default);
    Task<LocationDto?> GetLocationByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<LocationDto> CreateLocationAsync(CreateLocationDto dto, CancellationToken cancellationToken = default);
    Task UpdateLocationAsync(int id, UpdateLocationDto dto, CancellationToken cancellationToken = default);
    Task DeleteLocationAsync(int id, CancellationToken cancellationToken = default);
}