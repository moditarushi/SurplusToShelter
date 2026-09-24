using RunAndReason2.API.Exceptions;
using RunAndReason2.API.Models.DTOs;
using RunAndReason2.API.Repositories.Interfaces;
using RunAndReason2.API.Services.Interfaces;

namespace RunAndReason2.API.Services.Implementations;

public class VehicleService : IVehicleService
{
    private readonly IVehicleRepository _vehicleRepository;

    public VehicleService(IVehicleRepository vehicleRepository)
    {
        _vehicleRepository = vehicleRepository;
    }

    public async Task<IEnumerable<VehicleDto>> GetAllVehiclesAsync(CancellationToken cancellationToken = default)
    {
        var vehicles = await _vehicleRepository.GetAllAsync(cancellationToken);
        return vehicles.Select(v => v.ToDto());
    }

    public async Task<VehicleDto?> GetVehicleByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(id, cancellationToken);
        return vehicle?.ToDto();
    }

    public async Task<VehicleDto> CreateVehicleAsync(CreateVehicleDto dto, CancellationToken cancellationToken = default)
    {
        var entity = dto.ToEntity();
        var created = await _vehicleRepository.AddAsync(entity, cancellationToken);
        return created.ToDto();
    }

    public async Task UpdateVehicleAsync(int id, UpdateVehicleDto dto, CancellationToken cancellationToken = default)
    {
        var existing = await _vehicleRepository.GetByIdAsync(id, cancellationToken);
        if (existing == null)
        {
            throw new NotFoundException("Vehicle", id);
        }

        dto.ApplyTo(existing);
        await _vehicleRepository.UpdateAsync(existing, cancellationToken);
    }

    public async Task DeleteVehicleAsync(int id, CancellationToken cancellationToken = default)
    {
        var exists = await _vehicleRepository.ExistsAsync(id, cancellationToken);
        if (!exists)
        {
            throw new NotFoundException("Vehicle", id);
        }

        await _vehicleRepository.DeleteAsync(id, cancellationToken);
    }
}