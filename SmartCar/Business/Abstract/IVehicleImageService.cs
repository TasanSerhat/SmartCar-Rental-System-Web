using Core.Utilities.Results;
using Entities.Concrete;
using System.Collections.Generic;

namespace Business.Abstract
{
    public interface IVehicleImageService
    {
        IResult Add(Microsoft.AspNetCore.Http.IFormFile file, VehicleImage vehicleImage);
        IResult Delete(VehicleImage vehicleImage);
        IResult Update(Microsoft.AspNetCore.Http.IFormFile file, VehicleImage vehicleImage);
        IDataResult<List<VehicleImage>> GetAll();
        IDataResult<List<VehicleImage>> GetByVehicleId(int vehicleId);
        IDataResult<VehicleImage> GetById(int imageId);
    }
}
