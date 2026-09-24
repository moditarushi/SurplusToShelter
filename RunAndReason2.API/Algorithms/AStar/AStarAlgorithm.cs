using System.Diagnostics;
using RunAndReason2.API.Algorithms.Common;

namespace RunAndReason2.API.Algorithms.AStar;

/// <summary>
/// A* algorithm: finds the shortest path between two nodes using a heuristic
/// to prioritize exploration toward the goal. f(n) = g(n) + h(n).
/// </summary>
public class AStarAlgorithm : IShortestPathAlgorithm
{
    private readonly IHeuristic _heuristic;

    public AStarAlgorithm(IHeuristic heuristic)
    {
        _heuristic = heuristic;
    }

    public string AlgorithmName => "astar";

    public PathResult FindShortestPath(Graph graph, int startNodeId, int endNodeId)
    {
        var stopwatch = Stopwatch.StartNew();

        var gScore = new Dictionary<int, double>();
        var fScore = new Dictionary<int, double>();
        var previous = new Dictionary<int, int?>();
        var visited = new HashSet<int>();
        var openSet = new PriorityQueue<int, double>();

        var (endLat, endLon) = graph.GetCoordinates(endNodeId);

        foreach (var nodeId in graph.NodeIds)
        {
            gScore[nodeId] = double.PositiveInfinity;
            fScore[nodeId] = double.PositiveInfinity;
            previous[nodeId] = null;
        }

        gScore[startNodeId] = 0;

        var (startLat, startLon) = graph.GetCoordinates(startNodeId);
        fScore[startNodeId] = _heuristic.Calculate(startLat, startLon, endLat, endLon);

        openSet.Enqueue(startNodeId, fScore[startNodeId]);

        while (openSet.Count > 0)
        {
            var currentNode = openSet.Dequeue();

            if (visited.Contains(currentNode))
            {
                continue;
            }

            if (currentNode == endNodeId)
            {
                break;
            }

            visited.Add(currentNode);

            foreach (var edge in graph.GetEdges(currentNode))
            {
                if (visited.Contains(edge.ToNodeId))
                {
                    continue;
                }

                var tentativeGScore = gScore[currentNode] + edge.Weight;

                if (tentativeGScore < gScore[edge.ToNodeId])
                {
                    previous[edge.ToNodeId] = currentNode;
                    gScore[edge.ToNodeId] = tentativeGScore;

                    var (nodeLat, nodeLon) = graph.GetCoordinates(edge.ToNodeId);
                    var h = _heuristic.Calculate(nodeLat, nodeLon, endLat, endLon);
                    fScore[edge.ToNodeId] = tentativeGScore + h;

                    openSet.Enqueue(edge.ToNodeId, fScore[edge.ToNodeId]);
                }
            }
        }

        stopwatch.Stop();

        if (!gScore.ContainsKey(endNodeId) || double.IsPositiveInfinity(gScore[endNodeId]))
        {
            return new PathResult
            {
                PathFound = false,
                ExecutionTimeMs = stopwatch.ElapsedMilliseconds
            };
        }

        var path = BuildPath(previous, startNodeId, endNodeId);

        return new PathResult
        {
            PathFound = true,
            NodePath = path,
            TotalDistance = gScore[endNodeId],
            ExecutionTimeMs = stopwatch.ElapsedMilliseconds
        };
    }

    private static List<int> BuildPath(Dictionary<int, int?> previous, int startNodeId, int endNodeId)
    {
        var path = new List<int>();
        int? current = endNodeId;

        while (current != null)
        {
            path.Add(current.Value);
            current = previous[current.Value];
        }

        path.Reverse();

        return path[0] == startNodeId ? path : new List<int>();
    }
}