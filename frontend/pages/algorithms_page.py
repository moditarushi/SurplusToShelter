from dash import html
import dash_bootstrap_components as dbc

ALGORITHM_INFO = [
    {
        "name": "Dijkstra",
        "type": "Shortest Path",
        "description": "Finds the shortest path between two points in a graph by exploring outward from the start node, always expanding the closest unvisited node next.",
        "strengths": "Guaranteed optimal path. Simple, predictable, well-understood.",
        "weaknesses": "Explores in all directions equally — slower than A* when the goal direction is known.",
        "complexity": "Time: O((V + E) log V) with a priority queue. Space: O(V).",
    },
    {
        "name": "A* (A-Star)",
        "type": "Shortest Path",
        "description": "Like Dijkstra, but uses a heuristic to estimate remaining distance to the goal, prioritizing exploration toward the destination. f(n) = g(n) + h(n).",
        "strengths": "Faster than Dijkstra in practice when a good heuristic exists. Still optimal if the heuristic never overestimates.",
        "weaknesses": "Needs a well-chosen heuristic; a poor one can make it slower than Dijkstra.",
        "complexity": "Time: O((V + E) log V) worst case, typically much faster in practice. Space: O(V).",
    },
    {
        "name": "Nearest Neighbor",
        "type": "Route Optimization",
        "description": "A greedy baseline: starting at the depot, always travel to the closest unvisited customer next, until all are visited, then return to depot.",
        "strengths": "Extremely fast, simple to understand, good baseline for comparison.",
        "weaknesses": "Often produces noticeably suboptimal routes — greedy choices early on can force long detours later.",
        "complexity": "Time: O(n²) for n locations. Space: O(n).",
    },
    {
        "name": "Genetic Algorithm",
        "type": "Route Optimization",
        "description": "Maintains a population of candidate routes, evolving them over generations through selection, crossover, and mutation to converge on better solutions.",
        "strengths": "Can escape local optima that greedy methods get stuck in; tunable via population size, mutation rate, generations.",
        "weaknesses": "Slower per-run than greedy methods; result quality depends on parameter tuning.",
        "complexity": "Time: O(generations × population × n). Space: O(population × n).",
    },
    {
        "name": "Ant Colony Optimization",
        "type": "Route Optimization",
        "description": "Simulates ants depositing pheromone trails on good routes, with future ants more likely to follow stronger trails, reinforcing good paths over iterations.",
        "strengths": "Good for dynamically balancing exploration and exploitation of route choices.",
        "weaknesses": "More parameters to tune than GA; convergence can be slower.",
        "complexity": "Time: O(iterations × ants × n²). Space: O(n²) for the pheromone matrix.",
    },
]


def render_algorithms_page():
    cards = []
    for algo in ALGORITHM_INFO:
        badge_color = "info" if algo["type"] == "Shortest Path" else "success"
        cards.append(
            html.Div(
                [
                    html.Div(
                        [
                            html.Span(algo["name"], style={"fontSize": "1.2rem", "fontWeight": "700"}),
                            dbc.Badge(algo["type"], color=badge_color, className="ms-2"),
                        ]
                    ),
                    html.P(algo["description"], className="mt-2 mb-2", style={"color": "#94a3b8"}),
                    html.Div([html.Strong("Strengths: "), algo["strengths"]], className="mb-1"),
                    html.Div([html.Strong("Weaknesses: "), algo["weaknesses"]], className="mb-1"),
                    html.Div([html.Strong("Complexity: "), algo["complexity"]], className="mb-1"),
                ],
                className="rr-card",
            )
        )

    return html.Div([html.H2("Algorithms", className="mb-4"), html.Div(cards)])