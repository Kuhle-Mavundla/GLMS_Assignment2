using GLMS_Assignment2.Patterns.Factory;

namespace GLMS_Assignment2.Patterns.Observer
{
    // Observer that triggers invoice generation when a shipment status changes
    public class BillingDepartment : IShipmentObserver
    {
        public string LastNotification { get; private set; } = "";

        public void Update(Shipment shipment, string statusUpdate)
        {
            LastNotification = $"[BILLING] Shipment #{shipment.ShipmentID}: {statusUpdate} — Generate invoice.";
        }
    }
}