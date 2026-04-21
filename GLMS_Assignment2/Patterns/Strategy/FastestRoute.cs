namespace GLMS_Assignment2.Patterns.Strategy
{
    // Calculates the fastest (quickest delivery) route
    public class FastestRoute : IRoutingStrategy
    {
        public string CalculateRoute(string origin, string destination)
        {
            return $"Fastest Route: {origin} via Express Highway to {destination} (Estimated: 2 days)";
        }
    }
}