using Microsoft.AspNetCore.Mvc;
using RunAndReason2.API.Algorithms.Common;
using RunAndReason2.API.Models.DTOs;
using RunAndReason2.API.Repositories.Interfaces;
using RunAndReason2.API.Models.Entities;
namespace RunAndReason2.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OptimizationController : ControllerBase
{
    private readonly AlgorithmFactory _algorithmFactory;
    private readonly ILocationRepository _locationRepository;

    public OptimizationController(AlgorithmFactory algorithmFactory, ILocationRepository locationRepository)
    {
        _algorithmFactory = algorithmFactory;
        _locationRepository = locationRepository;
    }

    [HttpGet("algorithms")]
    public ActionResult<IEnumerable<string>> GetAvailableAlgorithms()
    {
        return Ok(_algorithmFactory.GetAvailableAlgorithmNames());
    }

    [HttpPost("run")]
    public async Task<ActionResult<OptimizationResultDto>> Run(OptimizationRequestDto request, CancellationToken cancellationToken)
    {
        var depot = await _locationRepository.GetByIdAsync(request.DepotId, cancellationToken);
        if (depot == null)
        {
            return NotFound($"Depot with id {request.DepotId} was not found.");
        }

        var allLocations = new List<Models.Entities.Location> { depot };

        foreach (var customerId in request.CustomerLocationIds)
        {
            var customer = await _locationRepository.GetByIdAsync(customerId, cancellationToken);
            if (customer == null)
            {
                return NotFound($"Customer location with id {customerId} was not found.");
            }
            allLocations.Add(customer);
        }

        try
        {
            var algorithm = _algorithmFactory.GetRouteOptimizationAlgorithm(request.Algorithm);
            var distanceMatrix = DistanceMatrixBuilder.Build(allLocations);

            var config = new RunAndReason2.API.Algorithms.Common.RouteOptimizationConfig
            {
                PopulationSize = request.Configuration?.PopulationSize ?? 100,
                MutationRate = request.Configuration?.MutationRate ?? 0.05,
                CrossoverRate = request.Configuration?.CrossoverRate ?? 0.8,
                Generations = request.Configuration?.Generations ?? 500,
                ElitismCount = request.Configuration?.ElitismCount ?? 2
            };

            var result = algorithm.OptimizeRoute(request.DepotId, request.CustomerLocationIds, distanceMatrix, config);

            return Ok(new OptimizationResultDto
            {
                Algorithm = algorithm.AlgorithmName,
                RouteOrder = result.LocationOrder,
                TotalDistanceKm = result.TotalDistance,
                ExecutionTimeMs = result.ExecutionTimeMs,
                ConvergenceHistory = result.ConvergenceHistory
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("shortest-path")]
    public async Task<ActionResult<ShortestPathResultDto>> ShortestPath(ShortestPathRequestDto request, CancellationToken cancellationToken)
    {
        var allLocationsResult = await _locationRepository.GetAllAsync(cancellationToken);
        var allLocations = allLocationsResult.ToList();

        if (!allLocations.Any(l => l.Id == request.StartLocationId))
        {
            return NotFound($"Start location with id {request.StartLocationId} was not found.");
        }

        if (!allLocations.Any(l => l.Id == request.EndLocationId))
        {
            return NotFound($"End location with id {request.EndLocationId} was not found.");
        }

        try
        {
            var algorithm = _algorithmFactory.GetShortestPathAlgorithm(request.Algorithm);
            var graph = GraphBuilder.Build(allLocations);

            var result = algorithm.FindShortestPath(graph, request.StartLocationId, request.EndLocationId);

            return Ok(new ShortestPathResultDto
            {
                Algorithm = algorithm.AlgorithmName,
                NodePath = result.NodePath,
                ExploredOrder = result.ExploredOrder,
                TotalDistanceKm = result.TotalDistance,
                ExecutionTimeMs = result.ExecutionTimeMs,
                PathFound = result.PathFound
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}