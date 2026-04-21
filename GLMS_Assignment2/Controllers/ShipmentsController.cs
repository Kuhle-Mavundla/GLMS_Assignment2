using Microsoft.AspNetCore.Mvc;
using GLMS.Web.Models.Enums;
using GLMS.Web.Patterns.Factory;
using GLMS.Web.Patterns.Strategy;
using GLMS.Web.Patterns.Observer;
using GLMS.Web.ViewModels;

namespace GLMS.Web.Controllers
{
    /// <summary>
    /// Demonstrates all three GoF design patterns integrated into the MVC app.
    /// </summary>
    public class ShipmentsController : Controller
    {
        public IActionResult Demo() => View(new ShipmentDemoViewModel());

        [HttpPost]
        public IActionResult Demo(ShipmentDemoViewModel vm)
        {
            // 1. FACTORY METHOD — create shipment based on type
            ShipmentFactory factory = vm.ShipmentType switch
            {
                ShipmentType.Air => new AirShipmentFactory(),
                ShipmentType.Land => new LandShipmentFactory(),
                ShipmentType.Sea => new SeaShipmentFactory(),
                _ => new AirShipmentFactory()
            };
            var shipment = factory.CreateShipment(1, vm.Origin, vm.Destination, vm.Cost);
            vm.ShipmentDetails = shipment.GetShipmentDetails();

            // 2. STRATEGY PATTERN — select routing strategy at runtime
            IRoutingStrategy strategy = vm.RoutingStrategy switch
            {
                "LeastCost" => new LeastCostRoute(),
                "EcoFriendly" => new EcoFriendlyRoute(),
                _ => new FastestRoute()
            };
            shipment.SetStrategy(strategy);
            vm.RoutingResult = shipment.PerformRouting();

            // 3. OBSERVER PATTERN — notify all observers
            var customer = new CustomerNotification();
            var warehouse = new WarehouseManager();
            var billing = new BillingDepartment();

            shipment.RegisterObserver(customer);
            shipment.RegisterObserver(warehouse);
            shipment.RegisterObserver(billing);

            shipment.Notify("Arrived at Customs");

            vm.Notifications = new List<string>
            {
                customer.LastNotification,
                warehouse.LastNotification,
                billing.LastNotification
            };

            return View(vm);
        }
    }
}