using Core.Entities;
using System;

namespace Entities.DTOs;

public class ReservationDetailDto : IDto
{
    public int ReservationId { get; set; }
    public string CustomerName { get; set; }
    public string VehiclePlate { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; }
}
