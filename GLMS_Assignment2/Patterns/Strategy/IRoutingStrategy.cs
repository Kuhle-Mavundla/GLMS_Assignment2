namespace GLMS.Web.Patterns.Strategy
{
    // STRATEGY PATTERN (Gang of Four)
    //
    // This interface defines a family of routing algorithms.
    // Each implementation provides a different way to calculate a route.
    // The algorithm can be swapped at runtime without changing the calling code.
    public interface IRoutingStrategy
    {
        string CalculateRoute(string origin, string destination);
    }
}