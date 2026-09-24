import os
import requests
from dotenv import load_dotenv

load_dotenv()

API_BASE_URL = os.getenv("API_BASE_URL", "http://localhost:5123")


class ApiClient:
    """
    Thin wrapper around the requests library for calling the RunAndReason2 API.
    All HTTP details (base URL, headers, error handling) live here so
    the rest of the frontend never talks to `requests` directly.
    """

    def __init__(self, base_url: str = API_BASE_URL):
        self.base_url = base_url.rstrip("/")

    def get(self, path: str, params: dict = None):
        url = f"{self.base_url}{path}"
        response = requests.get(url, params=params, timeout=10)
        response.raise_for_status()
        return response.json() if response.content else None

    def post(self, path: str, json_body: dict = None):
        url = f"{self.base_url}{path}"
        response = requests.post(url, json=json_body, timeout=10)
        response.raise_for_status()
        return response.json() if response.content else None

    def put(self, path: str, json_body: dict = None):
        url = f"{self.base_url}{path}"
        response = requests.put(url, json=json_body, timeout=10)
        response.raise_for_status()
        return response.json() if response.content else None

    def delete(self, path: str):
        url = f"{self.base_url}{path}"
        response = requests.delete(url, timeout=10)
        response.raise_for_status()
        return None


api_client = ApiClient()