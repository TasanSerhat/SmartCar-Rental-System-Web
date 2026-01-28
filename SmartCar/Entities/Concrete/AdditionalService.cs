using Core.Entities;

namespace Entities.Concrete;

public class AdditionalService : IEntity
{
    public int ServiceId { get; set; }
    public string ServiceName { get; set; }
    public string Description { get; set; }
    public decimal Cost { get; set; }
}
