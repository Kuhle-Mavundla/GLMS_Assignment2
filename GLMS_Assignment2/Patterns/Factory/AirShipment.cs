namespace GLMS.Web.Patterns.Factory
{
    // Concrete shipment class for air transport.
    // Created by the AirShipmentFactory.
    public class AirShipment : Shipment
    {
        public override string GetShipmentDetails()
        {
            return $"[AIR] Shipment #{ShipmentID}: {Origin} to {Destination} | Cost: {Cost:C}";
        }
    }
}