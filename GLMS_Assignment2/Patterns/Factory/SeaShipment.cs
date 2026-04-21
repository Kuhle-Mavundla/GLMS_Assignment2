namespace GLMS.Web.Patterns.Factory
{
    // Concrete shipment class for sea transport (cargo ship).
    // Created by the SeaShipmentFactory.
    public class SeaShipment : Shipment
    {
        public override string GetShipmentDetails()
        {
            return $"[SEA] Shipment #{ShipmentID}: {Origin} to {Destination} | Cost: {Cost:C}";
        }
    }
}