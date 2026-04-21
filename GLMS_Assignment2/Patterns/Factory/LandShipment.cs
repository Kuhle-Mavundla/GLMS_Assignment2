namespace GLMS_Assignment2.Patterns.Factory
{
    // Concrete shipment class for land transport (truck or rail).
    // Created by the LandShipmentFactory.
    public class LandShipment : Shipment
    {
        public override string GetShipmentDetails()
        {
            return $"[LAND] Shipment #{ShipmentID}: {Origin} to {Destination} | Cost: {Cost:C}";
        }
    }
}