using System;
using System.Collections.Generic;
using System.Text;

namespace VehicleParts.Application.DTOs.PurchaseInvoices;

public class PurchaseInvoiceResponseDto
{
    public int Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; }
    public decimal TotalAmount { get; set; }
    public int VendorId { get; set; }
    public string VendorName { get; set; } = string.Empty;
    public List<PurchaseInvoiceItemResponseDto> Items { get; set; } = new();
}

public class PurchaseInvoiceItemResponseDto
{
    public int PartId { get; set; }
    public string PartName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal SubTotal { get; set; }
}{
    internal class PurchaseInvoiceResponseDto
    {
    }
}
