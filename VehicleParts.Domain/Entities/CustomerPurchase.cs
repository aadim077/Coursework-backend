using System;
using System.Collections.Generic;

namespace VehicleParts.Domain.Entities
{
    public class CustomerPurchase : BaseEntity
    {
        public string CustomerId { get; set; } = string.Empty;

        public AppUser Customer { get; set; } = null!;

        public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;

        public string ReferenceNumber { get; set; } = string.Empty;

        public decimal TotalAmount { get; set; }

        public string Status { get; set; } = "Completed";

        public ICollection<CustomerPurchaseItem> Items { get; set; } = new List<CustomerPurchaseItem>();
    }
}