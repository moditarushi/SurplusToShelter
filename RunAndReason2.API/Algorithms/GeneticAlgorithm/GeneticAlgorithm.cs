using System.Diagnostics;
using RunAndReason2.API.Algorithms.Common;

namespace RunAndReason2.API.Algorithms.GeneticAlgorithm;

/// <summary>
/// Genetic Algorithm for the Vehicle Routing Problem.
/// A chromosome is a permutation of customer IDs representing visit order.
/// Evolves a population of candidate routes over generations using
/// tournament selection, ordered crossover, and swap mutation.
/// </summary>
public class GeneticAlgorithm : IRouteOptimizationAlgorithm
{
    private readonly Random _random = new();

    public string AlgorithmName => "genetic";

    public RouteResult OptimizeRoute(
        int depotId,
        List<int> customerIds,
        Dictionary<(int, int), double> distanceMatrix,
        RouteOptimizationConfig config)
    {
        var stopwatch = Stopwatch.StartNew();
        var convergenceHistory = new List<double>();

        if (customerIds.Count <= 1)
        {
            // Nothing to optimize with 0 or 1 customers - just return the trivial route.
            var trivialRoute = new List<int> { depotId };
            trivialRoute.AddRange(customerIds);
            trivialRoute.Add(depotId);

            return new RouteResult
            {
                LocationOrder = trivialRoute,
                TotalDistance = CalculateRouteDistance(trivialRoute, distanceMatrix),
                ExecutionTimeMs = stopwatch.ElapsedMilliseconds,
                ConvergenceHistory = convergenceHistory
            };
        }

        var population = InitializePopulation(customerIds, config.PopulationSize);
        List<int> bestChromosome = population[0];
        double bestDistance = CalculateChromosomeDistance(bestChromosome, depotId, distanceMatrix);

        for (int generation = 0; generation < config.Generations; generation++)
        {
            var fitnessScores = population
                .Select(chromosome => CalculateChromosomeDistance(chromosome, depotId, distanceMatrix))
                .ToList();

            // Track the best solution found so far this generation.
            var minDistanceThisGen = fitnessScores.Min();
            if (minDistanceThisGen < bestDistance)
            {
                bestDistance = minDistanceThisGen;
                bestChromosome = population[fitnessScores.IndexOf(minDistanceThisGen)];
            }
            convergenceHistory.Add(bestDistance);

            var newPopulation = new List<List<int>>();

            // Elitism: carry the best N chromosomes unchanged.
            var elites = population
                .Zip(fitnessScores, (chromosome, distance) => (chromosome, distance))
                .OrderBy(x => x.distance)
                .Take(config.ElitismCount)
                .Select(x => new List<int>(x.chromosome))
                .ToList();

            newPopulation.AddRange(elites);

            // Fill the rest of the population via selection + crossover + mutation.
            while (newPopulation.Count < config.PopulationSize)
            {
                var parent1 = TournamentSelect(population, fitnessScores);
                var parent2 = TournamentSelect(population, fitnessScores);

                List<int> child;
                if (_random.NextDouble() < config.CrossoverRate)
                {
                    child = OrderedCrossover(parent1, parent2);
                }
                else
                {
                    child = new List<int>(parent1);
                }

                if (_random.NextDouble() < config.MutationRate)
                {
                    SwapMutate(child);
                }

                newPopulation.Add(child);
            }

            population = newPopulation;
        }

        var finalRoute = new List<int> { depotId };
        finalRoute.AddRange(bestChromosome);
        finalRoute.Add(depotId);

        stopwatch.Stop();

        return new RouteResult
        {
            LocationOrder = finalRoute,
            TotalDistance = bestDistance,
            ExecutionTimeMs = stopwatch.ElapsedMilliseconds,
            ConvergenceHistory = convergenceHistory
        };
    }

    private List<List<int>> InitializePopulation(List<int> customerIds, int populationSize)
    {
        var population = new List<List<int>>();

        for (int i = 0; i < populationSize; i++)
        {
            var shuffled = new List<int>(customerIds);
            Shuffle(shuffled);
            population.Add(shuffled);
        }

        return population;
    }

    private void Shuffle(List<int> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = _random.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    private double CalculateChromosomeDistance(List<int> chromosome, int depotId, Dictionary<(int, int), double> distanceMatrix)
    {
        var fullRoute = new List<int> { depotId };
        fullRoute.AddRange(chromosome);
        fullRoute.Add(depotId);

        return CalculateRouteDistance(fullRoute, distanceMatrix);
    }

    private double CalculateRouteDistance(List<int> route, Dictionary<(int, int), double> distanceMatrix)
    {
        double total = 0;
        for (int i = 0; i < route.Count - 1; i++)
        {
            total += GetDistance(distanceMatrix, route[i], route[i + 1]);
        }
        return total;
    }

    private double GetDistance(Dictionary<(int, int), double> matrix, int from, int to)
    {
        if (matrix.TryGetValue((from, to), out var distance))
        {
            return distance;
        }
        if (matrix.TryGetValue((to, from), out var reverseDistance))
        {
            return reverseDistance;
        }
        throw new InvalidOperationException($"No distance found between locations {from} and {to}.");
    }

    private List<int> TournamentSelect(List<List<int>> population, List<double> fitnessScores, int tournamentSize = 3)
    {
        List<int>? best = null;
        double bestDistance = double.PositiveInfinity;

        for (int i = 0; i < tournamentSize; i++)
        {
            int index = _random.Next(population.Count);
            if (fitnessScores[index] < bestDistance)
            {
                bestDistance = fitnessScores[index];
                best = population[index];
            }
        }

        return new List<int>(best!);
    }

    /// <summary>
    /// Ordered Crossover (OX): preserves a random slice from parent1 in place,
    /// then fills the remaining positions with parent2's order, skipping
    /// anything already present - guarantees the child remains a valid permutation.
    /// </summary>
    private List<int> OrderedCrossover(List<int> parent1, List<int> parent2)
    {
        int size = parent1.Count;
        var child = new int?[size];

        int start = _random.Next(size);
        int end = _random.Next(size);
        if (start > end)
        {
            (start, end) = (end, start);
        }

        for (int i = start; i <= end; i++)
        {
            child[i] = parent1[i];
        }

        int childIndex = 0;
        foreach (var gene in parent2)
        {
            if (child.Contains(gene))
            {
                continue;
            }

            while (childIndex >= start && childIndex <= end)
            {
                childIndex++;
            }

            if (childIndex >= size)
            {
                break;
            }

            child[childIndex] = gene;
            childIndex++;
        }

        return child.Select(x => x!.Value).ToList();
    }

    private void SwapMutate(List<int> chromosome)
    {
        if (chromosome.Count < 2)
        {
            return;
        }

        int i = _random.Next(chromosome.Count);
        int j = _random.Next(chromosome.Count);
        (chromosome[i], chromosome[j]) = (chromosome[j], chromosome[i]);
    }
}