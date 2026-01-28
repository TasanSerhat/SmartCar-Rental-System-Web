using Core.Utilities.Results;
using Entities.Concrete;
using System.Collections.Generic;

namespace Business.Abstract;

public interface IVehicleService
{
    IDataResult<List<Vehicle>> GetAll();
    IDataResult<Vehicle> GetById(int vehicleId);
    IDataResult<Vehicle> GetWithDetails(int vehicleId);
    IResult Add(Vehicle vehicle);
    IResult Update(Vehicle vehicle);
    IResult Delete(Vehicle vehicle);
}
