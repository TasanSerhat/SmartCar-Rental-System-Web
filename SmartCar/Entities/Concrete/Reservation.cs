using Core.Entities;
using System;
using System.Collections.Generic;

namespace Entities.Concrete;

public class Reservation : IEntity
{
    public int ReservationId { get; set; }
    public int CustomerId { get; set; }
    public int VehicleId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime ReservationDate { get; set; }
    public string? Status { get; set; } = "Active";

    public Customer? Customer { get; set; }
    public Vehicle? Vehicle { get; set; }
    
    // Navigation for joined services
    public ICollection<ReservationService> ReservationServices { get; set; }
}
