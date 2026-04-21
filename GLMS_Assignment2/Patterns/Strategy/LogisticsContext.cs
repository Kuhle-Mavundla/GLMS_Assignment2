namespace GLMS_Assignment2.Patterns.Strategy
{
    // The Context class that uses a routing strategy.
    // You can change the strategy at runtime based on the client's needs.
    // For example: a Premium client might get the FastestRoute,
    // while a Standard client gets the LeastCostRoute.
    public class LogisticsContext
    {
        private IRoutingStrategy _strategy;

        // Constructor requires an initial strategy
        public LogisticsContext(IRoutingStrategy strategy)
        {
            _strategy = strategy;
        }

        // Change the strategy at runtime
        public void SetStrategy(IRoutingStrategy strategy)
        {
            _strategy = strategy;
        }

        // Run the current strategy
        public string ExecuteRouting(string origin, string destination)
        {
            return _strategy.CalculateRoute(origin, destination);
        }
    }
}