import dash
import dash_bootstrap_components as dbc
from dash import html, dcc, callback, Output, Input, State
import plotly.graph_objects as go
from services.location_service import get_all_locations
from services.optimization_service import get_available_algorithms, run_optimization, run_shortest_path
from components.map_view import build_map
from components.trace_diagram import build_trace_diagram

SHORTEST_PATH_ALGORITHMS = ("dijkstra", "astar")
ROUTE_ALGORITHMS_SIMPLE = ("nearestneighbor",)
ROUTE_ALGORITHMS_CONFIGURABLE = ("genetic",)


def render_dashboard_page():
    try:
        locations = get_all_locations()
    except Exception as e:
        return html.Div(
            [
                html.H2("Dashboard", className="mb-4"),
                dbc.Alert(f"Could not reach the API: {e}", color="danger"),
            ]
        )

    try:
        all_algorithms = get_available_algorithms()
    except Exception:
        all_algorithms = []

    def label_for(a):
        if a == "astar":
            return "A*"
        if a == "genetic":
            return "Genetic Algorithm"
        if a == "nearestneighbor":
            return "Nearest Neighbor"
        return a.capitalize()

    algorithm_options = [{"label": label_for(a), "value": a} for a in all_algorithms]

    depots = [loc for loc in locations if loc.get("isDepot")]
    customers = [loc for loc in locations if not loc.get("isDepot")]
    all_location_options = [{"label": loc["name"], "value": loc["id"]} for loc in locations]
    depot_options = [{"label": d["name"], "value": d["id"]} for d in depots]
    customer_options = [{"label": c["name"], "value": c["id"]} for c in customers]

    map_container = html.Div(id="dashboard-map-container", children=build_map(locations))

    config_panel = html.Div(
        [
            html.Div("Algorithm Configuration", className="rr-section-title"),
            dbc.Row(
                [
                    dbc.Col(
                        [
                            html.Label("Algorithm", className="rr-metric-label"),
                            dcc.Dropdown(
                                id="algorithm-dropdown",
                                options=algorithm_options,
                                value=algorithm_options[0]["value"] if algorithm_options else None,
                                clearable=False,
                                style={"color": "#0f172a"},
                            ),
                        ],
                        width=4,
                    ),
                ]
            ),
            # Shortest-path inputs (Dijkstra / A*)
            html.Div(
                id="shortest-path-inputs",
                children=[
                    dbc.Row(
                        [
                            dbc.Col(
                                [
                                    html.Label("Start Location", className="rr-metric-label"),
                                    dcc.Dropdown(
                                        id="start-location-dropdown",
                                        options=all_location_options,
                                        value=all_location_options[0]["value"] if all_location_options else None,
                                        clearable=False,
                                        style={"color": "#0f172a"},
                                    ),
                                ],
                                width=4,
                            ),
                            dbc.Col(
                                [
                                    html.Label("End Location", className="rr-metric-label"),
                                    dcc.Dropdown(
                                        id="end-location-dropdown",
                                        options=all_location_options,
                                        value=all_location_options[-1]["value"] if len(all_location_options) > 1 else None,
                                        clearable=False,
                                        style={"color": "#0f172a"},
                                    ),
                                ],
                                width=4,
                            ),
                        ],
                        className="mt-3",
                    ),
                ],
                style={"display": "none"},
            ),
            # Route optimization inputs (Nearest Neighbor / Genetic) - depot + customers, shared by both
            html.Div(
                id="route-optimization-inputs",
                children=[
                    dbc.Row(
                        [
                            dbc.Col(
                                [
                                        html.Label("Shelter", className="rr-metric-label"),                                    dcc.Dropdown(
                                        id="depot-dropdown",
                                        options=depot_options,
                                        value=depot_options[0]["value"] if depot_options else None,
                                        clearable=False,
                                        style={"color": "#0f172a"},
                                    ),
                                ],
                                width=4,
                            ),
                            dbc.Col(
                                [
                                        html.Label("Restaurants", className="rr-metric-label"),                                    dcc.Dropdown(
                                        id="customers-dropdown",
                                        options=customer_options,
                                        value=[c["value"] for c in customer_options],
                                        multi=True,
                                        style={"color": "#0f172a"},
                                    ),
                                ],
                                width=4,
                            ),
                        ],
                        className="mt-3",
                    ),
                ],
                style={"display": "none"},
            ),
            # GA-specific config inputs - shown only when Genetic Algorithm is selected
            html.Div(
                id="ga-config-inputs",
                children=[
                    dbc.Row(
                        [
                            dbc.Col(
                                [
                                    html.Label("Population Size", className="rr-metric-label"),
                                    dcc.Input(id="ga-population", type="number", value=100, min=10, style={"width": "100%"}),
                                ],
                                width=2,
                            ),
                            dbc.Col(
                                [
                                    html.Label("Mutation Rate (%)", className="rr-metric-label"),
                                    dcc.Input(id="ga-mutation", type="number", value=5, min=0, max=100, style={"width": "100%"}),
                                ],
                                width=2,
                            ),
                            dbc.Col(
                                [
                                    html.Label("Crossover Rate (%)", className="rr-metric-label"),
                                    dcc.Input(id="ga-crossover", type="number", value=80, min=0, max=100, style={"width": "100%"}),
                                ],
                                width=2,
                            ),
                            dbc.Col(
                                [
                                    html.Label("Generations", className="rr-metric-label"),
                                    dcc.Input(id="ga-generations", type="number", value=200, min=10, style={"width": "100%"}),
                                ],
                                width=2,
                            ),
                            dbc.Col(
                                [
                                    html.Label("Elitism Count", className="rr-metric-label"),
                                    dcc.Input(id="ga-elitism", type="number", value=2, min=0, style={"width": "100%"}),
                                ],
                                width=2,
                            ),
                        ],
                        className="mt-3",
                    ),
                ],
                style={"display": "none"},
            ),
            dbc.Button(
                "RUN OPTIMIZATION",
                id="run-optimization-btn",
                color="primary",
                className="mt-3",
                n_clicks=0,
            ),
        ],
        className="rr-card mt-3",
    )

    results_panel = html.Div(id="optimization-results-panel", className="mt-3")
    convergence_panel = html.Div(id="convergence-panel", className="mt-3")
    trace_panel = html.Div(id="trace-panel", className="mt-3")
    return html.Div(
        [
            html.H2("Dashboard", className="mb-4"),
            html.Div(map_container, className="rr-card"),
            config_panel,
            results_panel,
            trace_panel,
            convergence_panel,
        ]
    )


@callback(
    Output("shortest-path-inputs", "style"),
    Output("route-optimization-inputs", "style"),
    Output("ga-config-inputs", "style"),
    Input("algorithm-dropdown", "value"),
)
def toggle_input_panels(algorithm):
    hidden = {"display": "none"}
    shown = {"display": "block"}

    if algorithm in SHORTEST_PATH_ALGORITHMS:
        return shown, hidden, hidden
    elif algorithm in ROUTE_ALGORITHMS_SIMPLE:
        return hidden, shown, hidden
    elif algorithm in ROUTE_ALGORITHMS_CONFIGURABLE:
        return hidden, shown, shown
    return hidden, hidden, hidden


@callback(
    Output("optimization-results-panel", "children"),
    Output("dashboard-map-container", "children"),
    Output("convergence-panel", "children"),
    Output("trace-panel", "children"),
    Input("run-optimization-btn", "n_clicks"),
    State("algorithm-dropdown", "value"),
    State("start-location-dropdown", "value"),
    State("end-location-dropdown", "value"),
    State("depot-dropdown", "value"),
    State("customers-dropdown", "value"),
    State("ga-population", "value"),
    State("ga-mutation", "value"),
    State("ga-crossover", "value"),
    State("ga-generations", "value"),
    State("ga-elitism", "value"),
    prevent_initial_call=True,
)
def handle_run_optimization(
    n_clicks, algorithm, start_id, end_id, depot_id, customer_ids,
    ga_population, ga_mutation, ga_crossover, ga_generations, ga_elitism,
):
    empty_convergence = html.Div()
    empty_trace = html.Div()

    if not algorithm:
        return dbc.Alert("Select an algorithm.", color="warning"), dash.no_update, empty_convergence, empty_trace

    all_locations = get_all_locations()

    if algorithm in SHORTEST_PATH_ALGORITHMS:
        if not start_id or not end_id:
            return dbc.Alert("Select a start and end location.", color="warning"), dash.no_update, empty_convergence, empty_trace
        if start_id == end_id:
            return dbc.Alert("Start and end locations must be different.", color="warning"), dash.no_update, empty_convergence, empty_trace

        try:
            result = run_shortest_path(algorithm, start_id, end_id)
        except Exception as e:
            return dbc.Alert(f"Shortest path failed: {e}", color="danger"), dash.no_update, empty_convergence, empty_trace

        if not result.get("pathFound"):
            return dbc.Alert("No path found between the selected locations.", color="warning"), dash.no_update, empty_convergence, empty_trace

        metrics = build_shortest_path_metrics(result)
        updated_map = build_map(all_locations, route_order=result["nodePath"])

        algo_label = "A*" if algorithm == "astar" else algorithm.capitalize()
        trace_diagram = build_trace_diagram(all_locations, result["exploredOrder"], result["nodePath"], algo_label)

        return metrics, updated_map, empty_convergence, trace_diagram

    elif algorithm in ROUTE_ALGORITHMS_SIMPLE or algorithm in ROUTE_ALGORITHMS_CONFIGURABLE:
        if not depot_id or not customer_ids:
            return dbc.Alert("Select a depot and at least one customer.", color="warning"), dash.no_update, empty_convergence, empty_trace

        config = None
        if algorithm in ROUTE_ALGORITHMS_CONFIGURABLE:
            config = {
                "populationSize": int(ga_population or 100),
                "mutationRate": (ga_mutation or 5) / 100.0,
                "crossoverRate": (ga_crossover or 80) / 100.0,
                "generations": int(ga_generations or 200),
                "elitismCount": int(ga_elitism or 2),
            }

        try:
            result = run_optimization(algorithm, depot_id, customer_ids, config)
        except Exception as e:
            return dbc.Alert(f"Optimization failed: {e}", color="danger"), dash.no_update, empty_convergence, empty_trace

        metrics = build_route_metrics(result)
        updated_map = build_map(all_locations, route_order=result["routeOrder"])

        convergence_chart = empty_convergence
        if result.get("convergenceHistory"):
            convergence_chart = build_convergence_chart(result["convergenceHistory"])

        return metrics, updated_map, convergence_chart, empty_trace

    return dbc.Alert("Unknown algorithm selected.", color="danger"), dash.no_update, empty_convergence, empty_trace


def build_shortest_path_metrics(result):
    return html.Div(
        [
            metric_card("Algorithm", result["algorithm"].capitalize() if result["algorithm"] != "astar" else "A*"),
            metric_card("Total Distance", f"{result['totalDistanceKm']:.2f} km"),
            metric_card("Execution Time", f"{result['executionTimeMs']} ms"),
            metric_card("Path", " \u2192 ".join(str(x) for x in result["nodePath"]), flex=2),
        ],
        style={"display": "flex", "gap": "1rem"},
    )


def build_route_metrics(result):
    return html.Div(
        [
            metric_card("Algorithm", result["algorithm"].capitalize()),
            metric_card("Total Distance", f"{result['totalDistanceKm']:.2f} km"),
            metric_card("Execution Time", f"{result['executionTimeMs']} ms"),
            metric_card("Route Order", " \u2192 ".join(str(x) for x in result["routeOrder"]), flex=2),
        ],
        style={"display": "flex", "gap": "1rem"},
    )


def build_convergence_chart(convergence_history):
    fig = go.Figure()
    fig.add_trace(
        go.Scatter(
            y=convergence_history,
            mode="lines",
            line=dict(color="#38bdf8", width=2),
            name="Best Distance",
        )
    )
    fig.update_layout(
        title="Convergence History (Best Distance per Generation)",
        xaxis_title="Generation",
        yaxis_title="Distance (km)",
        template="plotly_dark",
        paper_bgcolor="#1e293b",
        plot_bgcolor="#1e293b",
        margin=dict(l=40, r=20, t=50, b=40),
        height=350,
    )

    return html.Div([dcc.Graph(figure=fig)], className="rr-card")


def metric_card(label, value, flex=1):
    return html.Div(
        [
            html.Div(label, className="rr-metric-label"),
            html.Div(value, className="rr-metric-value", style={"fontSize": "1.2rem"} if flex > 1 else {}),
        ],
        className="rr-card",
        style={"flex": flex},
    )