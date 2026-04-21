using GLMS_Assignment2.Models.Enums;

namespace GLMS_Assignment2.ViewModels
{
    // ViewModel for the design pattern demo page.
    // Shows how Factory, Strategy, and Observer patterns work together.
    public class ShipmentDemoViewModel
    {
        public ShipmentType ShipmentType { get; set; }
        public string Origin { get; set; } = "";
        public string Destination { get; set; } = "";
        public decimal Cost { get; set; }
        public string RoutingStrategy { get; set; } = "Fastest";

        // Output fields (populated after form submission)
        public string? ShipmentDetails { get; set; }
        public string? RoutingResult { get; set; }
        public List<string>? Notifications { get; set; }
    }
}