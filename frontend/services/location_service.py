from services.api_client import api_client


def get_all_locations():
    return api_client.get("/api/Locations")


def get_location_by_id(location_id: int):
    return api_client.get(f"/api/Locations/{location_id}")


def create_location(name: str, latitude: float, longitude: float, demand: int, is_depot: bool):
    payload = {
        "name": name,
        "latitude": latitude,
        "longitude": longitude,
        "demand": demand,
        "isDepot": is_depot,
    }
    return api_client.post("/api/Locations", payload)


def update_location(location_id: int, name: str, latitude: float, longitude: float, demand: int, is_depot: bool):
    payload = {
        "name": name,
        "latitude": latitude,
        "longitude": longitude,
        "demand": demand,
        "isDepot": is_depot,
    }
    return api_client.put(f"/api/Locations/{location_id}", payload)


def delete_location(location_id: int):
    return api_client.delete(f"/api/Locations/{location_id}")