using Core.Entities;
using System;

namespace Entities.Concrete;

public class Insurance : IEntity
{
    public int InsuranceId { get; set; }
    public int VehicleId { get; set; }
    public string PolicyType { get; set; }
    public string Provider { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public decimal Cost { get; set; }
    
    public Vehicle Vehicle { get; set; }
}
