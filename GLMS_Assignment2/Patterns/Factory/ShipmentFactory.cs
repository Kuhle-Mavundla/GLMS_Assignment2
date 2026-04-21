namespace GLMS.Web.Patterns.Factory
{
    // FACTORY METHOD PATTERN (Gang of Four)
    //
    // This abstract class defines a method for creating shipments.
    // Each concrete factory (Air, Land, Sea) decides which type to create.
    //
    // Why use this? If TechMove adds a new transport type (e.g. Drone),
    // we just create a new factory class without changing existing code.
    // This follows the Open-Closed Principle.
    public abstract class ShipmentFactory
    {
        public abstract Shipment CreateShipment(int id, string origin, string destination, decimal cost);
    }
}