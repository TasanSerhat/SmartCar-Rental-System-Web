using Core.Entities;

namespace Entities.Concrete;

public class Vehicle : IEntity
{
    public int VehicleId { get; set; }
    public string PlateNo { get; set; }
    public int ModelId { get; set; }
    public int BranchId { get; set; }
    public int ColorId { get; set; }
    // ModelYear and DailyPrice moved to VehicleModel
    public string Description { get; set; }
    public int Kilometer { get; set; }
    public string FuelType { get; set; }
    public string Status { get; set; } // Available, Rented, Maintenance

    public virtual VehicleModel? Model { get; set; }
    public virtual Branch? Branch { get; set; }
    public virtual Color? Color { get; set; }
    public virtual ICollection<VehicleImage>? VehicleImages { get; set; }
}
