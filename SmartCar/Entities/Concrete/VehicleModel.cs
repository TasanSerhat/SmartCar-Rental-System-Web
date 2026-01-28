using Core.Entities;

namespace Entities.Concrete;

public class VehicleModel : IEntity
{
    public int ModelId { get; set; }
    public string Brand { get; set; }
    public string ModelName { get; set; }
    public int Year { get; set; }
    public string TransmissionType { get; set; }
    public decimal PricePerDay { get; set; }
}
