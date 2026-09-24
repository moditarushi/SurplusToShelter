import dash_leaflet as dl
from dash import html


def build_location_markers(locations):
    """
    Converts a list of location dicts (from the API) into Leaflet markers.
    Depots are shown in a distinct color from customers.
    """
    markers = []

    for loc in locations:
        is_depot = loc.get("isDepot", False)
        color = "#38bdf8" if is_depot else "#4ade80"  # accent blue for depot, green for customers

        if is_depot:
            label = f"{loc['name']} (Depot)"
        else:
            label = f"{loc['name']} (Demand: {loc['demand']})"

        markers.append(
            dl.CircleMarker(
                center=[loc["latitude"], loc["longitude"]],
                radius=10 if is_depot else 7,
                color=color,
                fillColor=color,
                fillOpacity=0.8,
                children=[dl.Tooltip(label)],
            )
        )

    return markers


def build_route_polyline(locations_by_id, route_order):
    """
    Draws a line connecting locations in the order given by route_order (a list of location IDs).
    Used once optimization results exist (Phase 19+).
    """
    if not route_order:
        return None

    positions = []
    for loc_id in route_order:
        loc = locations_by_id.get(loc_id)
        if loc:
            positions.append([loc["latitude"], loc["longitude"]])

    if len(positions) < 2:
        return None

    return dl.Polyline(positions=positions, color="#fbbf24", weight=3, opacity=0.8)


def build_map(locations, route_order=None, height="500px"):
    """
    Builds a full Leaflet map centered on the average of all given locations.
    """
    if not locations:
        center = [26.9124, 75.7873]  # Jaipur city center
        zoom = 12.5
    else:
        avg_lat = sum(loc["latitude"] for loc in locations) / len(locations)
        avg_lon = sum(loc["longitude"] for loc in locations) / len(locations)
        center = [avg_lat, avg_lon]
        zoom = 12.5

    layers = [
        dl.TileLayer(url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"),
    ]

    layers.extend(build_location_markers(locations))

    if route_order:
        locations_by_id = {loc["id"]: loc for loc in locations}
        polyline = build_route_polyline(locations_by_id, route_order)
        if polyline:
            layers.append(polyline)

    return dl.Map(
        children=layers,
        center=center,
        zoom=zoom,
        style={"width": "100%", "height": height, "borderRadius": "10px"},
    )