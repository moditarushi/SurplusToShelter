using RunAndReason2.API.Data;
using RunAndReason2.API.Models.Entities;
using RunAndReason2.API.Repositories.Interfaces;

namespace RunAndReason2.API.Repositories.Implementations;

public class LocationRepository : RepositoryBase<Location>, ILocationRepository
{
    public LocationRepository(EFContext context) : base(context)
    {
    }
}