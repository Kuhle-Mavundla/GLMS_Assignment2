using GLMS.Web.Patterns.Factory;

namespace GLMS.Web.Patterns.Observer
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