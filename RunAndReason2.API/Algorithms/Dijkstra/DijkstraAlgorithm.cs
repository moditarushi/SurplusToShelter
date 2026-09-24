using System.Diagnostics;
using RunAndReason2.API.Algorithms.Common;

namespace RunAndReason2.API.Algorithms.Dijkstra;

public class DijkstraAlgorithm : IShortestPathAlgorithm
{
    public string AlgorithmName => "dijkstra";

    public PathResult FindShortestPath(Graph graph, int startNodeId, int endNodeId)
    {
        var stopwatch = Stopwatch.StartNew();

        var distances = new Dictionary<int, double>();
        var previous = new Dictionary<int, int?>();
        var visited = new HashSet<int>();
        var exploredOrder = new List<int>();
        var unvisited = new PriorityQueue<int, double>();

        foreach (var nodeId in graph.NodeIds)
        {
            distances[nodeId] = double.PositiveInfinity;
            previous[nodeId] = null;
        }

        distances[startNodeId] = 0;
        unvisited.Enqueue(startNodeId, 0);

        while (unvisited.Count > 0)
        {
            var currentNode = unvisited.Dequeue();

            if (visited.Contains(currentNode))
            {
                continue;
            }

            visited.Add(currentNode);
            exploredOrder.Add(currentNode);

            if (currentNode == endNodeId)
            {
                break;
            }

            foreach (var edge in graph.GetEdges(currentNode))
            {
                if (visited.Contains(edge.ToNodeId))
                {
                    continue;
                }

                var newDistance = distances[currentNode] + edge.Weight;

                if (newDistance < distances[edge.ToNodeId])
                {
                    distances[edge.ToNodeId] = newDistance;
                    previous[edge.ToNodeId] = currentNode;
                    unvisited.Enqueue(edge.ToNodeId, newDistance);
                }
            }
        }

        stopwatch.Stop();

        if (!distances.ContainsKey(endNodeId) || double.IsPositiveInfinity(distances[endNodeId]))
        {
            return new PathResult
            {
                PathFound = false,
                ExploredOrder = exploredOrder,
                ExecutionTimeMs = stopwatch.ElapsedMilliseconds
            };
        }

        var path = BuildPath(previous, startNodeId, endNodeId);

        return new PathResult
        {
            PathFound = true,
            NodePath = path,
            ExploredOrder = exploredOrder,
            TotalDistance = distances[endNodeId],
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