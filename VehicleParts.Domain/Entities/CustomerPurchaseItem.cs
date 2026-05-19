namespace VehicleParts.Domain.Entities
{
    public class CustomerPurchaseItem : BaseEntity
    {
        public int CustomerPurchaseId { get; set; }

        public CustomerPurchase CustomerPurchase { get; set; } = null!;

        public int? PartId { get; set; }

        public Part? Part { get; set; }

        public string ItemName { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal LineTotal { get; set; }
    }
}