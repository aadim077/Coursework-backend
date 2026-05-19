namespace VehicleParts.Application.DTOs.CustomerHistory
{
    public class PurchaseHistoryItemDto
    {
        public int Id { get; set; }

        public int? PartId { get; set; }

        public string ItemName { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal LineTotal { get; set; }
    }
}