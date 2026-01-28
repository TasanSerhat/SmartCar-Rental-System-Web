using Core.Entities;
using System;

namespace Entities.Concrete;

public class VehicleImage : IEntity
{
    public int Id { get; set; }
    public int VehicleId { get; set; }
    public string ImagePath { get; set; }
    public DateTime Date { get; set; }

    public Vehicle Vehicle { get; set; }
}
