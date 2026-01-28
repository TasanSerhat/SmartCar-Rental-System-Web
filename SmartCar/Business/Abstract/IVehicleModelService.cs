using Core.Utilities.Results;
using Entities.Concrete;

namespace Business.Abstract
{
    public interface IVehicleModelService
    {
        IDataResult<List<VehicleModel>> GetAll();
        IDataResult<VehicleModel> GetById(int modelId);
        IResult Add(VehicleModel vehicleModel);
        IResult Update(VehicleModel vehicleModel);
        IResult Delete(VehicleModel vehicleModel);
    }
}
