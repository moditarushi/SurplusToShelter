using RunAndReason2.API.Models.Entities;

namespace RunAndReason2.API.Algorithms.Common;

public static class DistanceMatrixBuilder
{
    public static Dictionary<(int, int), double> Build(List<Location> locations)
    {
        var matrix = new Dictionary<(int, int), double>();

        for (int i = 0; i < locations.Count; i++)
        {
            for (int j = i + 1; j < locations.Count; j++)
            {
                var a = locations[i];
                var b = locations[j];

                var distance = DistanceCalculator.HaversineDistanceKm(
                    a.Latitude, a.Longitude, b.Latitude, b.Longitude);

                matrix[(a.Id, b.Id)] = distance;
            }
        }

        return matrix;
    }
}