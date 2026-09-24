namespace RunAndReason2.API.Algorithms.Common;

public class AlgorithmFactory
{
    private readonly IEnumerable<IOptimizationAlgorithm> _algorithms;

    public AlgorithmFactory(IEnumerable<IOptimizationAlgorithm> algorithms)
    {
        _algorithms = algorithms;
    }

    public IShortestPathAlgorithm GetShortestPathAlgorithm(string algorithmName)
    {
        var algorithm = _algorithms
            .OfType<IShortestPathAlgorithm>()
            .FirstOrDefault(a => a.AlgorithmName.Equals(algorithmName, StringComparison.OrdinalIgnoreCase));

        if (algorithm == null)
        {
            throw new ArgumentException($"Unknown shortest-path algorithm: '{algorithmName}'.");
        }

        return algorithm;
    }

    public IRouteOptimizationAlgorithm GetRouteOptimizationAlgorithm(string algorithmName)
    {
        var algorithm = _algorithms
            .OfType<IRouteOptimizationAlgorithm>()
            .FirstOrDefault(a => a.AlgorithmName.Equals(algorithmName, StringComparison.OrdinalIgnoreCase));

        if (algorithm == null)
        {
            throw new ArgumentException($"Unknown route optimization algorithm: '{algorithmName}'.");
        }

        return algorithm;
    }

    public IEnumerable<string> GetAvailableAlgorithmNames()
    {
        return _algorithms.Select(a => a.AlgorithmName);
    }
}