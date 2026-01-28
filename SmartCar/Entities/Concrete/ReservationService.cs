using Core.Entities;

namespace Entities.Concrete;

public class ReservationService : IEntity
{
    public int ReservationId { get; set; }
    public int ServiceId { get; set; }
    
    public Reservation? Reservation { get; set; }
    public AdditionalService? Service { get; set; }
}
