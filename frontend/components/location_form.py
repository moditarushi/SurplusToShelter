import dash_bootstrap_components as dbc
from dash import html, dcc


def build_location_form():
    return html.Div(
        [
            html.Div("Add Location", className="rr-section-title"),
            dbc.Row(
                [
                    dbc.Col(
                        [
                            html.Label("Name", className="rr-metric-label"),
                            dcc.Input(
                                id="loc-form-name",
                                type="text",
                                placeholder="e.g. Laxmi Misthan Bhandar",
                                style={"width": "100%"},
                                className="mb-2",
                            ),
                        ],
                        width=3,
                    ),
                    dbc.Col(
                        [
                            html.Label("Latitude", className="rr-metric-label"),
                            dcc.Input(
                                id="loc-form-lat",
                                type="number",
                                placeholder="26.9124",
                                style={"width": "100%"},
                                className="mb-2",
                            ),
                        ],
                        width=2,
                    ),
                    dbc.Col(
                        [
                            html.Label("Longitude", className="rr-metric-label"),
                            dcc.Input(
                                id="loc-form-lon",
                                type="number",
                                placeholder="75.7873",
                                style={"width": "100%"},
                                className="mb-2",
                            ),
                        ],
                        width=2,
                    ),
                    dbc.Col(
                        [
                            html.Label("Demand", className="rr-metric-label"),
                            dcc.Input(
                                id="loc-form-demand",
                                type="number",
                                value=0,
                                min=0,
                                style={"width": "100%"},
                                className="mb-2",
                            ),
                        ],
                        width=2,
                    ),
                    dbc.Col(
                        [
                            html.Label("Type", className="rr-metric-label"),
                            dcc.Dropdown(
                                id="loc-form-is-depot",
                                options=[
                                    {"label": "Restaurant", "value": "false"},
                                    {"label": "Shelter", "value": "true"},
                                ],
                                value="false",
                                clearable=False,
                                style={"color": "#0f172a"},
                            ),
                        ],
                        width=2,
                    ),
                    dbc.Col(
                        [
                            html.Label("\u00A0", className="rr-metric-label d-block"),
                            dbc.Button("Add", id="loc-form-submit-btn", color="primary", n_clicks=0),
                        ],
                        width=1,
                    ),
                ],
                className="align-items-end",
            ),
            html.Div(id="loc-form-feedback", className="mt-2"),
        ],
        className="rr-card",
    )