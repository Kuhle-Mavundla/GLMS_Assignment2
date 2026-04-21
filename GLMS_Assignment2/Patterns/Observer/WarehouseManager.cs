using GLMS_Assignment2.Patterns.Factory;

namespace GLMS_Assignment2.Patterns.Observer
{
    // Observer that notifies the warehouse to prepare for incoming shipments
    public class WarehouseManager : IShipmentObserver
    {
        public string LastNotification { get; private set; } = "";

        public void Update(Shipment shipment, string statusUpdate)
        {
            LastNotification = $"[WAREHOUSE] Shipment #{shipment.ShipmentID}: {statusUpdate} — Prepare bay.";
        }
    }
}