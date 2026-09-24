namespace RunAndReason2.API.Algorithms.Common;

public interface IRouteOptimizationAlgorithm : IOptimizationAlgorithm
{
    RouteResult OptimizeRoute(
        int depotId,
        List<int> customerIds,
        Dictionary<(int, int), double> distanceMatrix,
        RouteOptimizationConfig config);
}