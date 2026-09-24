import dash_bootstrap_components as dbc
from dash import html, dash_table, callback, Output, Input, State
from services.location_service import get_all_locations, create_location
from components.map_view import build_map
from components.location_form import build_location_form


def render_locations_page():
    return html.Div(
        [
            html.H2("Locations", className="mb-4"),
            build_location_form(),
            html.Div(id="locations-content"),
        ]
    )


def build_locations_content():
    try:
        locations = get_all_locations()
        error_message = None
    except Exception as e:
        locations = []
        error_message = str(e)

    if error_message:
        return dbc.Alert(f"Could not reach the API: {error_message}", color="danger")

    if not locations:
        return dbc.Alert("No locations yet. Add one using the form above.", color="info")

    map_component = build_map(locations)

    table = dash_table.DataTable(
        columns=[
            {"name": "ID", "id": "id"},
            {"name": "Name", "id": "name"},
            {"name": "Latitude", "id": "latitude"},
            {"name": "Longitude", "id": "longitude"},
            {"name": "Demand", "id": "demand"},
            {"name": "Is Shelter", "id": "isDepot"},
            ],
        style_header={
            "backgroundColor": "#1e293b",
            "color": "#f1f5f9",
            "fontWeight": "600",
            "border": "1px solid #334155",
        },
        style_cell={
            "backgroundColor": "#0f172a",
            "color": "#f1f5f9",
            "border": "1px solid #334155",
            "padding": "10px",
        },
        style_table={"overflowX": "auto"},
    )

    return html.Div(
        [
            html.Div(map_component, className="rr-card"),
            html.Div(table, className="rr-card"),
        ]
    )


@callback(
    Output("locations-content", "children"),
    Input("locations-content", "id"),  # fires once on page load
)
def load_locations_content(_):
    return build_locations_content()


@callback(
    Output("loc-form-feedback", "children"),
    Output("locations-content", "children", allow_duplicate=True),
    Input("loc-form-submit-btn", "n_clicks"),
    State("loc-form-name", "value"),
    State("loc-form-lat", "value"),
    State("loc-form-lon", "value"),
    State("loc-form-demand", "value"),
    State("loc-form-is-depot", "value"),
    prevent_initial_call=True,
)
def handle_add_location(n_clicks, name, lat, lon, demand, is_depot_str):
    if not name or lat is None or lon is None:
        return dbc.Alert("Name, latitude, and longitude are required.", color="warning"), build_locations_content()

    is_depot = is_depot_str == "true"

    try:
        create_location(
            name=name,
            latitude=float(lat),
            longitude=float(lon),
            demand=int(demand) if demand else 0,
            is_depot=is_depot,
        )
    except Exception as e:
        return dbc.Alert(f"Failed to add location: {e}", color="danger"), build_locations_content()

    return dbc.Alert(f"'{name}' added successfully.", color="success"), build_locations_content()