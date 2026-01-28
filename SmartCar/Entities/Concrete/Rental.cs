using Core.Entities;

namespace Entities.Concrete;

public class Rental : IEntity
{
    public int RentalId { get; set; }
    public int ReservationId { get; set; }
    public int PaymentId { get; set; }
    public int TotalDays { get; set; }
    public decimal TotalCost { get; set; }
    
    public Reservation? Reservation { get; set; }
    public Payment? Payment { get; set; }
}
