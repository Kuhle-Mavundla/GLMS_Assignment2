namespace GLMS.Web.Patterns.Factory
{
    // Concrete factory that creates LandShipment objects
    public class LandShipmentFactory : ShipmentFactory
    {
        public override Shipment CreateShipment(int id, string origin, string destination, decimal cost)
        {
            return new LandShipment
            {
                ShipmentID = id,
                Origin = origin,
                Destination = destination,
                Cost = cost
            };
        }
    }
}