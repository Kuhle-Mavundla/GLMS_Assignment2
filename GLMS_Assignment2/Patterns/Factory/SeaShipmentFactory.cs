namespace GLMS_Assignment2.Patterns.Factory
{
    // Concrete factory that creates SeaShipment objects
    public class SeaShipmentFactory : ShipmentFactory
    {
        public override Shipment CreateShipment(int id, string origin, string destination, decimal cost)
        {
            return new SeaShipment
            {
                ShipmentID = id,
                Origin = origin,
                Destination = destination,
                Cost = cost
            };
        }
    }
}