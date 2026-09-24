namespace RunAndReason2.API.Algorithms.Common;

/// <summary>
/// For algorithms that find the shortest path between two specific nodes in a graph.
/// </summary>
public interface IShortestPathAlgorithm : IOptimizationAlgorithm
{
    PathResult FindShortestPath(Graph graph, int startNodeId, int endNodeId);
}