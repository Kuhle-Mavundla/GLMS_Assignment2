namespace GLMS.Web.Patterns.Strategy
{
    // Calculates the most environmentally friendly route (low carbon footprint)
    public class EcoFriendlyRoute : IRoutingStrategy
    {
        public string CalculateRoute(string origin, string destination)
        {
            return $"Eco-Friendly Route: {origin} via Green Logistics Path to {destination} (Estimated: 5 days, Low Carbon)";
        }
    }
}