using Business.Abstract;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;

namespace Business.Concrete
{
    public class VehicleModelManager : IVehicleModelService
    {
        IVehicleModelDal _vehicleModelDal;

        public VehicleModelManager(IVehicleModelDal vehicleModelDal)
        {
            _vehicleModelDal = vehicleModelDal;
        }

        public IResult Add(VehicleModel vehicleModel)
        {
            _vehicleModelDal.Add(vehicleModel);
            return new SuccessResult("VehicleModel added successfully");
        }

        public IResult Delete(VehicleModel vehicleModel)
        {
            _vehicleModelDal.Delete(vehicleModel);
            return new SuccessResult("VehicleModel deleted successfully");
        }

        public IDataResult<List<VehicleModel>> GetAll()
        {
            return new SuccessDataResult<List<VehicleModel>>(_vehicleModelDal.GetAll());
        }

        public IDataResult<VehicleModel> GetById(int modelId)
        {
            return new SuccessDataResult<VehicleModel>(_vehicleModelDal.Get(v => v.ModelId == modelId));
        }

        public IResult Update(VehicleModel vehicleModel)
        {
            _vehicleModelDal.Update(vehicleModel);
            return new SuccessResult("VehicleModel updated successfully");
        }
    }
}
