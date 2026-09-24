namespace RunAndReason2.API.Algorithms.Common;

/// <summary>
/// A simple weighted graph representation shared by shortest-path algorithms.
/// Nodes are identified by integer IDs (matching Location IDs).
/// </summary>
public class Graph
{
    private readonly Dictionary<int, List<Edge>> _adjacency = new();
    private readonly Dictionary<int, (double Latitude, double Longitude)> _coordinates = new();

    public IReadOnlyCollection<int> NodeIds => _adjacency.Keys;

    public void AddNode(int nodeId, double latitude = 0, double longitude = 0)
    {
        if (!_adjacency.ContainsKey(nodeId))
        {
            _adjacency[nodeId] = new List<Edge>();
        }

        _coordinates[nodeId] = (latitude, longitude);
    }

    public void AddEdge(int fromNodeId, int toNodeId, double weight)
    {
        AddNode(fromNodeId);
        AddNode(toNodeId);

        // Undirected: distance from A to B equals B to A for our use case (Euclidean distance).
        _adjacency[fromNodeId].Add(new Edge(toNodeId, weight));
        _adjacency[toNodeId].Add(new Edge(fromNodeId, weight));
    }

    public IReadOnlyList<Edge> GetEdges(int nodeId)
    {
        return _adjacency.TryGetValue(nodeId, out var edges) ? edges : new List<Edge>();
    }

    public (double Latitude, double Longitude) GetCoordinates(int nodeId)
    {
        return _coordinates.TryGetValue(nodeId, out var coords) ? coords : (0, 0);
    }
}

public record Edge(int ToNodeId, double Weight);