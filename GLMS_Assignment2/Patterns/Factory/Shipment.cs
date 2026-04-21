using GLMS.Web.Patterns.Strategy;
using GLMS.Web.Patterns.Observer;

namespace GLMS.Web.Patterns.Factory
{
    // This is the base class for all shipment types (Air, Land, Sea).
    // It is abstract, meaning you cannot create a Shipment directly —
    // you must use one of the concrete classes (AirShipment, LandShipment, etc.)
    //
    // This class also connects to the Strategy and Observer patterns:
    // - Strategy: different routing algorithms can be swapped at runtime
    // - Observer: multiple departments are notified when the shipment status changes
    public abstract class Shipment
    {
        public int ShipmentID { get; set; }
        public string Origin { get; set; } = "";
        public string Destination { get; set; } = "";
        public decimal Cost { get; set; }

        // STRATEGY PATTERN: holds the current routing strategy
        private IRoutingStrategy? _strategy;

        // OBSERVER PATTERN: list of observers watching this shipment
        private readonly List<IShipmentObserver> _observers = new();

        // Set which routing strategy to use
        public void SetStrategy(IRoutingStrategy strategy)
        {
            _strategy = strategy;
        }

        // Execute the routing calculation using the current strategy
        public string PerformRouting()
        {
            if (_strategy == null)
                throw new InvalidOperationException("No routing strategy has been set.");
            return _strategy.CalculateRoute(Origin, Destination);
        }

        // Add an observer that wants to be notified of status changes
        public void RegisterObserver(IShipmentObserver observer)
        {
            _observers.Add(observer);
        }

        // Remove an observer so it stops receiving notifications
        public void RemoveObserver(IShipmentObserver observer)
        {
            _observers.Remove(observer);
        }

        // Notify all registered observers about a status change
        public void Notify(string statusUpdate)
        {
            foreach (var observer in _observers)
            {
                observer.Update(this, statusUpdate);
            }
        }

        // Each shipment type must provide its own implementation of this method
        public abstract string GetShipmentDetails();
    }
}