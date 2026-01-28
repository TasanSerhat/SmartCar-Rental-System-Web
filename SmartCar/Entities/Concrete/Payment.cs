using Core.Entities;
using System;

namespace Entities.Concrete;

public class Payment : IEntity
{
    public int PaymentId { get; set; }
    public int RentalId { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public string PaymentMethod { get; set; }
    
    public Rental Rental { get; set; }
}
