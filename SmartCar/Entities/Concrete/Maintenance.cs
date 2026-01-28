using Core.Entities;
using System;

namespace Entities.Concrete;

public class Maintenance : IEntity
{
    // Composite Key: VehicleId + MaintenanceId
    public int VehicleId { get; set; }
    public int MaintenanceId { get; set; }
    
    public DateTime Date { get; set; }
    public string Description { get; set; }
    public decimal Cost { get; set; }
    
    public Vehicle Vehicle { get; set; }
}
