namespace GLMS.Web.Patterns.Factory
{
    // Concrete factory that creates AirShipment objects
    public class AirShipmentFactory : ShipmentFactory
    {
        public override Shipment CreateShipment(int id, string origin, string destination, decimal cost)
        {
            return new AirShipment
            {
                ShipmentID = id,
                Origin = origin,
                Destination = destination,
                Cost = cost
            };
        }
    }
}