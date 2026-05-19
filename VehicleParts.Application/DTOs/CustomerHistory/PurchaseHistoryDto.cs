using System;
using System.Collections.Generic;

namespace VehicleParts.Application.DTOs.CustomerHistory
{
    public class PurchaseHistoryDto
    {
        public int Id { get; set; }

        public DateTime PurchaseDate { get; set; }

        public string ReferenceNumber { get; set; } = string.Empty;

        public decimal TotalAmount { get; set; }

        public string Status { get; set; } = string.Empty;

        public List<PurchaseHistoryItemDto> Items { get; set; } = new List<PurchaseHistoryItemDto>();
    }
}