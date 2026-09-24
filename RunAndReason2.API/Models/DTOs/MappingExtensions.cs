using RunAndReason2.API.Models.Entities;

namespace RunAndReason2.API.Models.DTOs;

public static class MappingExtensions
{
    public static LocationDto ToDto(this Location location)
    {
        return new LocationDto
        {
            Id = location.Id,
            Name = location.Name,
            Latitude = location.Latitude,
            Longitude = location.Longitude,
            Demand = location.Demand,
            IsDepot = location.IsDepot
        };
    }

    public static Location ToEntity(this CreateLocationDto dto)
    {
        return new Location
        {
            Name = dto.Name,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            Demand = dto.Demand,
            IsDepot = dto.IsDepot
        };
    }

    public static void ApplyTo(this UpdateLocationDto dto, Location location)
    {
        location.Name = dto.Name;
        location.Latitude = dto.Latitude;
        location.Longitude = dto.Longitude;
        location.Demand = dto.Demand;
        location.IsDepot = dto.IsDepot;
    }

    public static VehicleDto ToDto(this Vehicle vehicle)
    {
        return new VehicleDto
        {
            Id = vehicle.Id,
            Name = vehicle.Name,
            Capacity = vehicle.Capacity,
            StartingDepotId = vehicle.StartingDepotId,
            IsActive = vehicle.IsActive
        };
    }

    public static Vehicle ToEntity(this CreateVehicleDto dto)
    {
        return new Vehicle
        {
            Name = dto.Name,
            Capacity = dto.Capacity,
            StartingDepotId = dto.StartingDepotId,
            IsActive = true
        };
    }

    public static void ApplyTo(this UpdateVehicleDto dto, Vehicle vehicle)
    {
        vehicle.Name = dto.Name;
        vehicle.Capacity = dto.Capacity;
        vehicle.StartingDepotId = dto.StartingDepotId;
        vehicle.IsActive = dto.IsActive;
    }
}