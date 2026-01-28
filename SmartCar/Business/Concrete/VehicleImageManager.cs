using Business.Abstract;
using Core.Utilities.Helpers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Business.Concrete
{
    public class VehicleImageManager : IVehicleImageService
    {
        IVehicleImageDal _vehicleImageDal;
        IFileHelper _fileHelper;

        public VehicleImageManager(IVehicleImageDal vehicleImageDal, IFileHelper fileHelper)
        {
            _vehicleImageDal = vehicleImageDal;
            _fileHelper = fileHelper;
        }

        public IResult Add(Microsoft.AspNetCore.Http.IFormFile file, VehicleImage vehicleImage)
        {
            var result = CheckIfVehicleImageLimitExceeded(vehicleImage.VehicleId);
            if (!result.Success)
            {
                return result;
            }

            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            var fileName = _fileHelper.Upload(file, imagePath + "\\");
            
            vehicleImage.ImagePath = "/images/" + fileName;
            vehicleImage.Date = DateTime.UtcNow;

            _vehicleImageDal.Add(vehicleImage);
            return new SuccessResult();
        }

        public IResult Delete(VehicleImage vehicleImage)
        {
            var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", vehicleImage.ImagePath.TrimStart('/'));
            _fileHelper.Delete(oldPath);
            
            _vehicleImageDal.Delete(vehicleImage);
            return new SuccessResult();
        }

        public IDataResult<List<VehicleImage>> GetAll()
        {
            return new SuccessDataResult<List<VehicleImage>>(_vehicleImageDal.GetAll());
        }

        public IDataResult<VehicleImage> GetById(int imageId)
        {
            return new SuccessDataResult<VehicleImage>(_vehicleImageDal.Get(v => v.Id == imageId));
        }

        public IDataResult<List<VehicleImage>> GetByVehicleId(int vehicleId)
        {
            var result = _vehicleImageDal.GetAll(v => v.VehicleId == vehicleId);
            if (result.Count == 0)
            {
                // Ensure default image path valid
                return new SuccessDataResult<List<VehicleImage>>(new List<VehicleImage> 
                { 
                    new VehicleImage { VehicleId = vehicleId, Date = DateTime.UtcNow, ImagePath = "/images/default.jpg" } 
                });
            }
            return new SuccessDataResult<List<VehicleImage>>(result);
        }

        public IResult Update(Microsoft.AspNetCore.Http.IFormFile file, VehicleImage vehicleImage)
        {
            var oldImage = _vehicleImageDal.Get(v => v.Id == vehicleImage.Id);
            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            var oldFullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", oldImage.ImagePath.TrimStart('/'));
            
            var newFileName = _fileHelper.Update(file, oldFullPath, imagePath + "\\");
            
            vehicleImage.ImagePath = "/images/" + newFileName;
            vehicleImage.Date = DateTime.UtcNow;
            
            _vehicleImageDal.Update(vehicleImage);
            return new SuccessResult();
        }

        private IResult CheckIfVehicleImageLimitExceeded(int vehicleId)
        {
            var result = _vehicleImageDal.GetAll(v => v.VehicleId == vehicleId).Count;
            if (result >= 5)
            {
                return new ErrorResult("A vehicle can have at most 5 images");
            }
            return new SuccessResult();
        }
    }
}
