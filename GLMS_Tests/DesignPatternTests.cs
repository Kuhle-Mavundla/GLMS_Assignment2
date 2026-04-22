using Xunit;
using GLMS_Assignment2.Patterns.Factory;
using GLMS_Assignment2.Patterns.Strategy;
using GLMS_Assignment2.Patterns.Observer;

namespace GLMS_Tests
{
    /// <summary>
    /// Tests for the three GoF design patterns: Factory Method, Strategy, Observer.
    /// Demonstrates TDD principles by testing pattern behavior.
    /// </summary>
    public class DesignPatternTests
    {
        // ── FACTORY METHOD TESTS ──
        [Fact]
        public void AirShipmentFactory_CreatesAirShipment()
        {
            var factory = new AirShipmentFactory();
            var shipment = factory.CreateShipment(1, "Johannesburg", "London", 5000m);

            Assert.IsType<AirShipment>(shipment);
            Assert.Equal(1, shipment.ShipmentID);
            Assert.Equal("Johannesburg", shipment.Origin);
            Assert.Equal("London", shipment.Destination);
            Assert.Equal(5000m, shipment.Cost);
        }

        [Fact]
        public void LandShipmentFactory_CreatesLandShipment()
        {
            var factory = new LandShipmentFactory();
            var shipment = factory.CreateShipment(2, "Cape Town", "Durban", 1500m);
            Assert.IsType<LandShipment>(shipment);
            Assert.Contains("[LAND]", shipment.GetShipmentDetails());
        }

        [Fact]
        public void SeaShipmentFactory_CreatesSeaShipment()
        {
            var factory = new SeaShipmentFactory();
            var shipment = factory.CreateShipment(3, "Durban", "Mumbai", 8000m);
            Assert.IsType<SeaShipment>(shipment);
            Assert.Contains("[SEA]", shipment.GetShipmentDetails());
        }

        [Fact]
        public void ShipmentDetails_ContainsOriginAndDestination()
        {
            var factory = new AirShipmentFactory();
            var shipment = factory.CreateShipment(1, "Paris", "Tokyo", 9000m);
            var details = shipment.GetShipmentDetails();

            Assert.Contains("Paris", details);
            Assert.Contains("Tokyo", details);
        }

        // ── STRATEGY PATTERN TESTS ──
        [Fact]
        public void FastestRoute_ReturnsExpressRoute()
        {
            var strategy = new FastestRoute();
            var result = strategy.CalculateRoute("A", "B");
            Assert.Contains("Fastest", result);
            Assert.Contains("A", result);
            Assert.Contains("B", result);
        }

        [Fact]
        public void LeastCostRoute_ReturnsEconomyRoute()
        {
            var strategy = new LeastCostRoute();
            var result = strategy.CalculateRoute("A", "B");
            Assert.Contains("Least-Cost", result);
        }

        [Fact]
        public void EcoFriendlyRoute_ReturnsGreenRoute()
        {
            var strategy = new EcoFriendlyRoute();
            var result = strategy.CalculateRoute("A", "B");
            Assert.Contains("Eco-Friendly", result);
            Assert.Contains("Low Carbon", result);
        }

        [Fact]
        public void LogisticsContext_CanSwitchStrategies()
        {
            var context = new LogisticsContext(new FastestRoute());
            var result1 = context.ExecuteRouting("A", "B");
            Assert.Contains("Fastest", result1);

            context.SetStrategy(new LeastCostRoute());
            var result2 = context.ExecuteRouting("A", "B");
            Assert.Contains("Least-Cost", result2);
        }

        [Fact]
        public void Shipment_PerformRouting_WithNoStrategy_Throws()
        {
            var factory = new AirShipmentFactory();
            var shipment = factory.CreateShipment(1, "A", "B", 100m);
            Assert.Throws<InvalidOperationException>(() => shipment.PerformRouting());
        }

        [Fact]
        public void Shipment_PerformRouting_WithStrategy_ReturnsResult()
        {
            var factory = new AirShipmentFactory();
            var shipment = factory.CreateShipment(1, "Nairobi", "Lagos", 3000m);
            shipment.SetStrategy(new EcoFriendlyRoute());
            var result = shipment.PerformRouting();

            Assert.Contains("Nairobi", result);
            Assert.Contains("Lagos", result);
        }

        // ── OBSERVER PATTERN TESTS ──
        [Fact]
        public void Observer_CustomerNotification_ReceivesUpdate()
        {
            var factory = new AirShipmentFactory();
            var shipment = factory.CreateShipment(1, "A", "B", 100m);
            var customer = new CustomerNotification();

            shipment.RegisterObserver(customer);
            shipment.Notify("Arrived at Customs");

            Assert.Contains("CUSTOMER", customer.LastNotification);
            Assert.Contains("Arrived at Customs", customer.LastNotification);
        }

        [Fact]
        public void Observer_AllObserversNotified()
        {
            var factory = new SeaShipmentFactory();
            var shipment = factory.CreateShipment(5, "A", "B", 500m);
            var customer = new CustomerNotification();
            var warehouse = new WarehouseManager();
            var billing = new BillingDepartment();

            shipment.RegisterObserver(customer);
            shipment.RegisterObserver(warehouse);
            shipment.RegisterObserver(billing);

            shipment.Notify("Delivered");

            Assert.Contains("Delivered", customer.LastNotification);
            Assert.Contains("Delivered", warehouse.LastNotification);
            Assert.Contains("Delivered", billing.LastNotification);
        }

        [Fact]
        public void Observer_RemovedObserver_DoesNotReceiveUpdate()
        {
            var factory = new AirShipmentFactory();
            var shipment = factory.CreateShipment(1, "A", "B", 100m);
            var customer = new CustomerNotification();

            shipment.RegisterObserver(customer);
            shipment.RemoveObserver(customer);
            shipment.Notify("Status changed");

            Assert.Empty(customer.LastNotification);
        }

        [Fact]
        public void Observer_WarehouseManager_PrepareBayMessage()
        {
            var factory = new LandShipmentFactory();
            var shipment = factory.CreateShipment(10, "X", "Y", 200m);
            var warehouse = new WarehouseManager();

            shipment.RegisterObserver(warehouse);
            shipment.Notify("In Transit");

            Assert.Contains("Prepare bay", warehouse.LastNotification);
        }

        [Fact]
        public void Observer_BillingDepartment_GenerateInvoiceMessage()
        {
            var factory = new AirShipmentFactory();
            var shipment = factory.CreateShipment(7, "X", "Y", 300m);
            var billing = new BillingDepartment();

            shipment.RegisterObserver(billing);
            shipment.Notify("Delivered");

            Assert.Contains("Generate invoice", billing.LastNotification);
        }
    }
}