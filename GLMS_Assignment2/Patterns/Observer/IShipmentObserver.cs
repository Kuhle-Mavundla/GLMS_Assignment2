using GLMS.Web.Patterns.Factory;

namespace GLMS.Web.Patterns.Observer
{
    // OBSERVER PATTERN (Gang of Four)
    //
    // This interface is implemented by any class that wants to be notified
    // when a shipment's status changes (e.g. CustomerNotification, WarehouseManager).
    // The Shipment class (the Subject) keeps a list of observers and notifies them all.
    public interface IShipmentObserver
    {
        void Update(Shipment shipment, string statusUpdate);
    }
}