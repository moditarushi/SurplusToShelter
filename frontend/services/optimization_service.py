from services.api_client import api_client


def get_available_algorithms():
    return api_client.get("/api/optimization/algorithms")


def run_optimization(algorithm: str, depot_id: int, customer_location_ids: list, configuration: dict = None):
    payload = {
        "algorithm": algorithm,
        "depotId": depot_id,
        "customerLocationIds": customer_location_ids,
    }
    if configuration:
        payload["configuration"] = configuration
    return api_client.post("/api/optimization/run", payload)


def run_shortest_path(algorithm: str, start_location_id: int, end_location_id: int):
    payload = {
        "algorithm": algorithm,
        "startLocationId": start_location_id,
        "endLocationId": end_location_id,
    }
    return api_client.post("/api/optimization/shortest-path", payload)