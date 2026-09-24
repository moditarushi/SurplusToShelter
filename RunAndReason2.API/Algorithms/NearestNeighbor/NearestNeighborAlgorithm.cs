using System.Diagnostics;
using RunAndReason2.API.Algorithms.Common;

namespace RunAndReason2.API.Algorithms.NearestNeighbor;

/// <summary>
/// Greedy baseline: from the current location, always visit the nearest
/// unvisited customer next. Fast but often suboptimal - used as a simple
/// baseline to compare smarter algorithms (GA, ACO) against.
/// </summary>
public class NearestNeighborAlgorithm : IRouteOptimizationAlgorithm
{
    public string AlgorithmName => "nearestneighbor";

    public RouteResult OptimizeRoute(int depotId, List<int> customerIds, Dictionary<(int, int), double> distanceMatrix, RouteOptimizationConfig config)    {
        var stopwatch = Stopwatch.StartNew();

        var unvisited = new List<int>(customerIds);
        var route = new List<int> { depotId };
        var current = depotId;
        double totalDistance = 0;

        while (unvisited.Count > 0)
        {
            var nearest = unvisited
                .OrderBy(customerId => GetDistance(distanceMatrix, current, customerId))
                .First();

            totalDistance += GetDistance(distanceMatrix, current, nearest);
            route.Add(nearest);
            unvisited.Remove(nearest);
            current = nearest;
        }

        // Return to depot to complete the route.
        totalDistance += GetDistance(distanceMatrix, current, depotId);
        route.Add(depotId);

        stopwatch.Stop();

        return new RouteResult
        {
            LocationOrder = route,
            TotalDistance = totalDistance,
            ExecutionTimeMs = stopwatch.ElapsedMilliseconds,
            ConvergenceHistory = new List<double>() // Nearest Neighbor is a single-pass greedy algorithm, no convergence
        };
    }

    private static double GetDistance(Dictionary<(int, int), double> matrix, int from, int to)
    {
        if (matrix.TryGetValue((from, to), out var distance))
        {
            return distance;
        }

        // Matrix is symmetric; try the reverse pair before failing.
        if (matrix.TryGetValue((to, from), out var reverseDistance))
        {
            return reverseDistance;
        }

        throw new InvalidOperationException($"No distance found between locations {from} and {to}.");
    }
}