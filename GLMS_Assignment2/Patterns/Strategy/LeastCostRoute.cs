namespace GLMS_Assignment2.Patterns.Strategy
{
    // Calculates the cheapest route (saves money, takes longer)
    public class LeastCostRoute : IRoutingStrategy
    {
        public string CalculateRoute(string origin, string destination)
        {
            return $"Least-Cost Route: {origin} via Economy Corridor to {destination} (Estimated: 7 days)";
        }
    }
}