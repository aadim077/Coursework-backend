using Microsoft.AspNetCore.Mvc;
using VehicleParts.Domain.Entities;
using VehicleParts.Infrastructure.Data;
using System;
using System.Collections.Generic;

namespace coursework.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestEmailController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TestEmailController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("seed-data")]
        public async Task<IActionResult> SeedData()
        {
            // Create a fake vendor first to satisfy the foreign key
            var vendor = new Vendor
            {
                Name = "Test Vendor",
                ContactPerson = "Test Person",
                Email = "test@vendor.com",
                Phone = "123456789",
                Address = "Test Address"
            };
            _context.Vendors.Add(vendor);
            await _context.SaveChangesAsync();

            // 1. Seed a Low Stock Part (Stock = 2, Min = 10)
            var part = new Part
            {
                Name = "Test Spark Plug (Low Stock)",
                Description = "A test item to trigger low stock email",
                Category = "Engine",
                Price = 15.99m,
                StockQuantity = 2,
                MinimumStockLevel = 10,
                VendorId = vendor.Id
            };
            _context.Parts.Add(part);

            // 2. Seed an old Unpaid Invoice (> 1 month old)
            var invoice = new SalesInvoice
            {
                InvoiceNumber = "INV-TEST-001",
                CustomerName = "Test Customer",
                CustomerEmail = "aadimrai884@gmail.com", // Sending to you so you can see it
                TotalAmount = 450.00m,
                InvoiceDate = DateTime.UtcNow.AddMonths(-2), // 2 months ago
                IsPaid = false,
                Items = new List<SalesInvoiceItem>
                {
                    new SalesInvoiceItem { Part = part, Quantity = 1, UnitPrice = 450.00m, SubTotal = 450.00m }
                }
            };
            _context.SalesInvoices.Add(invoice);

            await _context.SaveChangesAsync();

            return Ok(new { message = "Test data successfully inserted! The background job should pick this up within 1 minute." });
        }
    }
}
