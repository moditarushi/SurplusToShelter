# 🍲 Surplus to Shelter

**A smart routing platform that plans the fastest way to collect surplus food from multiple restaurants across Jaipur and deliver it to a shelter — before it spoils.**

Built with an ASP.NET Core 8 backend and a Python (Dash) frontend, this project doesn't just draw a route — it lets you **watch and compare different routing algorithms think**, in real time, on an actual map of Jaipur.

---

## 🤔 The problem, in plain words

Every day, restaurants across Jaipur have leftover food that's still good to eat. A shelter wants to collect it and feed people who need it.

But here's the catch: there isn't just **one** restaurant to visit — there are **many**, scattered across the city. And someone has to decide:

> "If I have to visit 6 restaurants today and bring everything back to the shelter, what's the smartest order to visit them in, and what's the fastest road to take between each one?"

This is a much harder problem than it sounds. Visit them in the wrong order, and a volunteer could end up driving back and forth across the city, wasting time the food doesn't have.

That's the real problem this platform solves.

---

## 🗺️ How is this different from Google Maps?

This is the question most people ask first, so let's be direct about it.

| | **Google Maps** | **Surplus to Shelter** |
|---|---|---|
| **What it solves** | "Get me from Point A to Point B" | "Visit ALL these restaurants in the smartest order, then return" |
| **Number of stops** | Usually one destination (or a short manual list you order yourself) | Many restaurants at once — the app *decides the best order for you* |
| **What it optimizes for** | Fastest route between two fixed points | Shortest total path across the *entire* pickup route, so more food gets collected in less time |
| **What you see** | Just the final route | The final route **and** a live, step-by-step animation of *how* the algorithm figured it out |
| **Comparison** | One algorithm, hidden from you | Multiple algorithms, side by side, so you can see which one performs better and why |

In short: **Google Maps tells you how to get from A to B. This platform decides the best order to visit A, B, C, D, E, and F — and shows you exactly how different strategies "think" about that decision.**

That second problem (visiting many points in the best order) is called the **Vehicle Routing Problem**, and it's a genuinely different, harder challenge than simple point-to-point navigation.

---

## 🧠 Meet the algorithms — explained like you're new to this

This platform runs the same problem through **four different strategies**, so you can literally watch them compete.

### 1️⃣ Dijkstra's Algorithm — "The careful one"

**Question it answers:** *Between exactly two points, what is the guaranteed shortest road path?*

Think of Dijkstra as a very careful explorer. Starting from a restaurant, it checks every nearby road, then the next ring of roads, then the next — like ripples spreading out in a pond — until it reaches the destination. It never skips a check, so it's always 100% correct.

**Downside:** it double-checks roads that are obviously going the wrong way, which wastes a little time.

### 2️⃣ A* Algorithm — "The smart one"

**Question it answers:** Same as Dijkstra — but faster.

A* does the same search as Dijkstra, but it has a rough sense of direction (a straight-line estimate to the destination), so it explores *toward* the goal instead of checking every direction equally.

**On this platform, you can literally watch this difference** — there's a live animated diagram that lights up each road/intersection as the algorithm checks it. A* consistently lights up fewer points before finding the same correct answer Dijkstra finds. Same destination, less wasted effort.

### 3️⃣ Nearest Neighbor — "The impulsive one"

**Question it answers:** *Out of many restaurants, what order should I visit them in?*

This is where the *real* Google-Maps-can't-do-this problem starts. Nearest Neighbor is simple: from wherever you are right now, just go to whichever restaurant is closest. Repeat until you've visited everyone, then head back to the shelter.

It's fast to compute and often "good enough" — but it can make a shortsighted choice early on that forces a long, annoying detour later. It's included mainly as a quick baseline to compare smarter methods against.

### 4️⃣ Genetic Algorithm — "The one that learns from its mistakes"

**Question it answers:** Same as Nearest Neighbor — but tries much harder to find a genuinely better order.

Instead of picking one route and committing to it, the Genetic Algorithm starts with a big batch of random possible routes, then repeatedly:
- keeps the best ones,
- mixes pairs of good routes together (like genetic crossover),
- occasionally shuffles a route slightly at random (mutation),

...and repeats this hundreds of times, getting a little better each round. You can watch its **convergence graph** — a line that drops lower and lower as it discovers better routes over time. It usually beats Nearest Neighbor, at the cost of taking longer to compute.

---

## 🔗 How the four algorithms work together

```
                    ┌─────────────────────────────┐
                    │   Which restaurants to visit  │
                    │      and in what order?       │
                    └──────────────┬───────────────┘
                                   │
                    Nearest Neighbor  OR  Genetic Algorithm
                                   │
                                   ▼
                    ┌─────────────────────────────┐
                    │  What's the actual fastest    │
                    │  road between each stop?      │
                    └──────────────┬───────────────┘
                                   │
                       Dijkstra  OR  A*
                                   │
                                   ▼
                    ┌─────────────────────────────┐
                    │     Final delivery route      │
                    │   shown live on Jaipur map    │
                    └─────────────────────────────┘
```

This mirrors how real food-rescue dispatch actually works: one decision layer picks *who to visit and in what order*, and another decides *what road to actually drive* between each stop.

---

## 🏗️ System architecture

```
                     SURPLUS TO SHELTER
                            │
             ┌──────────────┴──────────────┐
             │                             │
             ▼                             ▼
       Python Frontend              ASP.NET Core API
     (Dash + interactive map)            (.NET 8)
                                            │
                              ┌─────────────┼─────────────┐
                              ▼             ▼             ▼
                         Controllers    Services       Repositories
                              │             │             │
                              └─────────────┼─────────────┘
                                            │
                              ┌─────────────┴─────────────┐
                              ▼                           ▼
                         SQL Server                   MongoDB
                 (restaurants, shelter,         (algorithm run history,
                     route data)              convergence data, experiments)
```

**Backend:** ASP.NET Core 8 Web API. Each algorithm is a self-contained, swappable module (Dijkstra, A*, Nearest Neighbor, Genetic Algorithm) plugged into the app through a common interface — so adding a new algorithm later doesn't require rewriting existing ones.

**Frontend:** A Python-based dashboard with a live map centered on Jaipur, algorithm selection, and animated visualizations showing each algorithm's thought process.

---

## 🛠️ Tech stack

| Layer | Technology |
|---|---|
| Backend API | ASP.NET Core 8, Entity Framework Core, MongoDB.Driver |
| Frontend | Python, Dash, Plotly, interactive Leaflet maps |
| Structured data | SQL Server |
| Algorithm history & research data | MongoDB |
| API documentation | Swagger / OpenAPI |

---

## ✅ What makes this a "research platform," not just an app

Most routing tools give you one answer and hide the reasoning. This platform is built around **transparency and comparison**:

- 🎬 Watch each shortest-path algorithm explore the map node by node, animated
- 📈 Watch the Genetic Algorithm's solution improve generation by generation
- ⚖️ Run the same restaurants through different algorithms and compare total distance, execution time, and route quality side by side
- 🧪 Run batches of experiments across different algorithm settings to see what configuration performs best

---

## 🚀 Getting started

### You'll need
- .NET 8 SDK
- Python 3.12
- SQL Server (running locally)
- MongoDB Server (running locally)

### Run the backend
```bash
cd RunAndReason2.API
copy appsettings.Development.json.example appsettings.Development.json
dotnet ef database update
dotnet run
```

### Run the frontend
```bash
cd frontend
py -3.12 -m venv venv
venv\Scripts\activate
pip install -r requirements.txt
copy .env.example .env
python app.py
```

Then open **http://127.0.0.1:8050** in your browser.

---

## 📍 Project status

**Working today:** interactive Jaipur map, Dijkstra, A*, Nearest Neighbor, Genetic Algorithm, live algorithm-exploration animations, convergence graphs.

**In progress:** batch experiment comparisons, Ant Colony Optimization.

---

## 📄 License

Personal research and learning project.
