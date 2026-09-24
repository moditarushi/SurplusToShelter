namespace RunAndReason2.API.Algorithms.Common;

/// <summary>
/// Common marker interface for all algorithms in the system.
/// Enables the Factory to look up any algorithm by its string name.
/// </summary>
public interface IOptimizationAlgorithm
{
    string AlgorithmName { get; }
}