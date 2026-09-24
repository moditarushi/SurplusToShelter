using Microsoft.AspNetCore.Mvc;
using RunAndReason2.API.Exceptions;
using RunAndReason2.API.Models.DTOs;
using RunAndReason2.API.Services.Interfaces;

namespace RunAndReason2.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LocationsController : ControllerBase
{
    private readonly ILocationService _locationService;

    public LocationsController(ILocationService locationService)
    {
        _locationService = locationService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LocationDto>>> GetAll(CancellationToken cancellationToken)
    {
        var locations = await _locationService.GetAllLocationsAsync(cancellationToken);
        return Ok(locations);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<LocationDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var location = await _locationService.GetLocationByIdAsync(id, cancellationToken);
        if (location == null)
        {
            return NotFound();
        }

        return Ok(location);
    }

    [HttpPost]
    public async Task<ActionResult<LocationDto>> Create(CreateLocationDto dto, CancellationToken cancellationToken)
    {
        var created = await _locationService.CreateLocationAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateLocationDto dto, CancellationToken cancellationToken)
    {
        try
        {
            await _locationService.UpdateLocationAsync(id, dto, cancellationToken);
            return NoContent();
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            await _locationService.DeleteLocationAsync(id, cancellationToken);
            return NoContent();
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }
}