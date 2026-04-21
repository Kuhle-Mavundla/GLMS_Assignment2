using GLMS.Web.Patterns.Factory;

namespace GLMS.Web.Patterns.Observer
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