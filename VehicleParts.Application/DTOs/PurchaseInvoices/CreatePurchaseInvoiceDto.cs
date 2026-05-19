using System;
using System.Collections.Generic;
using System.Text;

namespace VehicleParts.Application.DTOs.PurchaseInvoices;

public class CreatePurchaseInvoiceDto
{
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;
    public int VendorId { get; set; }
    public List<CreatePurchaseInvoiceItemDto> Items { get; set; } = new();
}

public class CreatePurchaseInvoiceItemDto
{
    public int PartId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}