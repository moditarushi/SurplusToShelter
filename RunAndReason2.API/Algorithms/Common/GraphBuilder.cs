using RunAndReason2.API.Models.Entities;

namespace RunAndReason2.API.Algorithms.Common;

public static class GraphBuilder
{
    /// <summary>
    /// Builds a graph where each location connects only to its K nearest neighbors,
    /// rather than every other location. This mimics a simplified road network,
    /// so shortest-path algorithms can meaningfully route through intermediate stops
    /// instead of always finding a trivial direct line.
    /// </summary>
    public static Graph Build(List<Location> locations, int nearestNeighborCount = 3)
    {
        var graph = new Graph();

        foreach (var location in locations)
        {
            graph.AddNode(location.Id, location.Latitude, location.Longitude);
        }

        foreach (var location in locations)
        {
            var nearest = locations
                .Where(other => other.Id != location.Id)
                .Select(other => new
                {
                    Location = other,
                    Distance = DistanceCalculator.HaversineDistanceKm(
                        location.Latitude, location.Longitude,
                        other.Latitude, other.Longitude)
                })
                .OrderBy(x => x.Distance)
                .Take(nearestNeighborCount);

            foreach (var neighbor in nearest)
            {
                graph.AddEdge(location.Id, neighbor.Location.Id, neighbor.Distance);
            }
        }

        return graph;
    }
}