using GLMS_Assignment2.Patterns.Factory;

namespace GLMS_Assignment2.Patterns.Observer
{
    // Observer that sends notifications to the customer
    // In a real system, this would send an email or push notification
    public class CustomerNotification : IShipmentObserver
    {
        // Stores the last notification for testing purposes
        public string LastNotification { get; private set; } = "";

        public void Update(Shipment shipment, string statusUpdate)
        {
            LastNotification = $"[CUSTOMER] Shipment #{shipment.ShipmentID}: {statusUpdate}";
        }
    }
}