using GLMS.Web.Patterns.Factory;

namespace GLMS.Web.Patterns.Observer
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