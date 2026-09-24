import dash
import dash_bootstrap_components as dbc
from dash import html, dcc, callback, Output, Input
from pages.locations_page import render_locations_page
from pages.dashboard_page import render_dashboard_page
from pages.algorithms_page import render_algorithms_page

app = dash.Dash(
    __name__,
    external_stylesheets=[dbc.themes.DARKLY],
    suppress_callback_exceptions=True,
    title="Surplus to Shelter")

sidebar = html.Div(
    [
        html.Div("SURPLUS TO SHELTER", className="rr-brand"),        html.Div(
            [
                dcc.Link("Dashboard", href="/", className="rr-nav-link d-block"),
                dcc.Link("Locations", href="/locations", className="rr-nav-link d-block"),
                dcc.Link("Vehicles", href="/vehicles", className="rr-nav-link d-block"),
                dcc.Link("Algorithms", href="/algorithms", className="rr-nav-link d-block"),
                dcc.Link("Experiments", href="/experiments", className="rr-nav-link d-block"),
                dcc.Link("Results", href="/results", className="rr-nav-link d-block"),
            ]
        ),
    ],
    className="rr-sidebar",
    style={"width": "220px", "position": "fixed", "top": 0, "left": 0, "bottom": 0},
)

content = html.Div(
    id="page-content",
    style={"marginLeft": "220px", "padding": "2rem"},
)

app.layout = html.Div([dcc.Location(id="url", refresh=False), sidebar, content])


@callback(Output("page-content", "children"), Input("url", "pathname"))
def render_page(pathname):
    if pathname == "/locations":
        return render_locations_page()
    elif pathname == "/algorithms":
        return render_algorithms_page()
    elif pathname in ("/vehicles", "/experiments", "/results"):
        return html.Div(
            [
                html.H2(pathname.strip("/").capitalize(), className="mb-4"),
                dbc.Alert("This page is coming in a later phase.", color="info"),
            ]
        )
    else:
        return render_dashboard_page()


if __name__ == "__main__":
    app.run(debug=True, port=8050)