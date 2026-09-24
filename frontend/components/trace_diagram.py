import plotly.graph_objects as go
from dash import html, dcc


def build_trace_diagram(locations, explored_order, node_path, algorithm_name):
    """
    Builds an animated node-link diagram showing the order in which
    a shortest-path algorithm explored nodes, then highlights the
    final path once exploration completes.
    """
    location_by_id = {loc["id"]: loc for loc in locations}

    # Use lat/lon as x/y positions directly - good enough for a schematic view.
    positions = {
        loc["id"]: (loc["longitude"], loc["latitude"]) for loc in locations
    }

    all_ids = list(location_by_id.keys())
    path_set = set(node_path)

    frames = []
    num_steps = len(explored_order)

    for step in range(1, num_steps + 1):
        visited_so_far = set(explored_order[:step])
        current_node = explored_order[step - 1]

        node_colors = []
        node_sizes = []
        for node_id in all_ids:
            if node_id == current_node:
                node_colors.append("#fbbf24")  # amber = currently being explored
                node_sizes.append(26)
            elif node_id in visited_so_far:
                node_colors.append("#38bdf8")  # blue = already explored
                node_sizes.append(20)
            else:
                node_colors.append("#334155")  # dark gray = not yet explored
                node_sizes.append(16)

        frame_data = [
            _build_edge_trace(location_by_id, dim=True),
            go.Scatter(
                x=[positions[nid][0] for nid in all_ids],
                y=[positions[nid][1] for nid in all_ids],
                mode="markers+text",
                marker=dict(size=node_sizes, color=node_colors, line=dict(width=2, color="#0f172a")),
                text=[location_by_id[nid]["name"] for nid in all_ids],
                textposition="top center",
                textfont=dict(color="#f1f5f9", size=10),
                hoverinfo="text",
            ),
        ]

        # On the final frame, overlay the highlighted shortest path.
        if step == num_steps and node_path:
            frame_data.append(_build_path_trace(location_by_id, node_path, positions))

        frames.append(go.Frame(data=frame_data, name=str(step)))

    # Initial figure state = first frame.
    initial_colors = ["#334155"] * len(all_ids)
    initial_sizes = [16] * len(all_ids)

    fig = go.Figure(
        data=[
            _build_edge_trace(location_by_id, dim=True),
            go.Scatter(
                x=[positions[nid][0] for nid in all_ids],
                y=[positions[nid][1] for nid in all_ids],
                mode="markers+text",
                marker=dict(size=initial_sizes, color=initial_colors, line=dict(width=2, color="#0f172a")),
                text=[location_by_id[nid]["name"] for nid in all_ids],
                textposition="top center",
                textfont=dict(color="#f1f5f9", size=10),
                hoverinfo="text",
            ),
        ],
        frames=frames,
    )

    fig.update_layout(
        title=f"{algorithm_name} - Node Exploration Trace",
        template="plotly_dark",
        paper_bgcolor="#1e293b",
        plot_bgcolor="#1e293b",
        height=450,
        margin=dict(l=20, r=20, t=50, b=20),
        xaxis=dict(visible=False),
        yaxis=dict(visible=False),
        updatemenus=[
            {
                "type": "buttons",
                "showactive": False,
                "y": 1.08,
                "x": 0,
                "xanchor": "left",
                "buttons": [
                    {
                        "label": "Play",
                        "method": "animate",
                        "args": [None, {"frame": {"duration": 600, "redraw": True}, "fromcurrent": True}],
                    },
                    {
                        "label": "Pause",
                        "method": "animate",
                        "args": [[None], {"frame": {"duration": 0, "redraw": False}, "mode": "immediate"}],
                    },
                ],
            }
        ],
        sliders=[
            {
                "steps": [
                    {
                        "args": [[str(step)], {"frame": {"duration": 0, "redraw": True}, "mode": "immediate"}],
                        "label": str(step),
                        "method": "animate",
                    }
                    for step in range(1, num_steps + 1)
                ],
                "x": 0,
                "y": -0.05,
                "currentvalue": {"prefix": "Step: ", "font": {"color": "#f1f5f9"}},
            }
        ],
    )

    return html.Div([dcc.Graph(figure=fig)], className="rr-card")


def _build_edge_trace(location_by_id, dim=False):
    """
    Draws faint lines between every pair of locations to show the underlying
    graph structure (context for why the algorithm might skip around).
    """
    edge_x = []
    edge_y = []
    ids = list(location_by_id.keys())

    for i in range(len(ids)):
        for j in range(i + 1, len(ids)):
            a = location_by_id[ids[i]]
            b = location_by_id[ids[j]]
            edge_x += [a["longitude"], b["longitude"], None]
            edge_y += [a["latitude"], b["latitude"], None]

    return go.Scatter(
        x=edge_x,
        y=edge_y,
        mode="lines",
        line=dict(width=0.5, color="#334155" if dim else "#64748b"),
        hoverinfo="none",
        showlegend=False,
    )


def _build_path_trace(location_by_id, node_path, positions):
    """
    Draws the final highlighted shortest path in amber/green, overlaid
    on top of everything else once exploration is complete.
    """
    path_x = [positions[nid][0] for nid in node_path]
    path_y = [positions[nid][1] for nid in node_path]

    return go.Scatter(
        x=path_x,
        y=path_y,
        mode="lines+markers",
        line=dict(width=4, color="#4ade80"),
        marker=dict(size=14, color="#4ade80"),
        hoverinfo="none",
        showlegend=False,
        name="Final Path",
    )