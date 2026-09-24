using RunAndReason2.API.Models.DTOs;

namespace RunAndReason2.API.Services.Interfaces;

public interface IVehicleService
{
    Task<IEnumerable<VehicleDto>> GetAllVehiclesAsync(CancellationToken cancellationToken = default);
    Task<VehicleDto?> GetVehicleByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<VehicleDto> CreateVehicleAsync(CreateVehicleDto dto, CancellationToken cancellationToken = default);
    Task UpdateVehicleAsync(int id, UpdateVehicleDto dto, CancellationToken cancellationToken = default);
    Task DeleteVehicleAsync(int id, CancellationToken cancellationToken = default);
}