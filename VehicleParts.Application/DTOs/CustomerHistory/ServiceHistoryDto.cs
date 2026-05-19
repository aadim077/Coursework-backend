using System;

namespace VehicleParts.Application.DTOs.CustomerHistory
{
    public class ServiceHistoryDto
    {
        public int AppointmentId { get; set; }
        public int VehicleId { get; set; }
        public string VehicleMake { get; set; } = string.Empty;
        public string VehicleModel { get; set; } = string.Empty;
        public string VehicleNumber { get; set; } = string.Empty;
        public DateTime AppointmentDateTime { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}

