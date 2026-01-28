using Business.Abstract;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using System.Collections.Generic;

namespace Business.Concrete;

public class AdditionalServiceManager : IAdditionalServiceService
{
    IAdditionalServiceDal _serviceDal;

    public AdditionalServiceManager(IAdditionalServiceDal serviceDal)
    {
        _serviceDal = serviceDal;
    }

    public IResult Add(AdditionalService service)
    {
        _serviceDal.Add(service);
        return new SuccessResult("Service added");
    }

    public IResult Delete(AdditionalService service)
    {
        _serviceDal.Delete(service);
        return new SuccessResult("Service deleted");
    }

    public IDataResult<List<AdditionalService>> GetAll()
    {
        return new SuccessDataResult<List<AdditionalService>>(_serviceDal.GetAll());
    }

    public IDataResult<AdditionalService> GetById(int id)
    {
        return new SuccessDataResult<AdditionalService>(_serviceDal.Get(s => s.ServiceId == id));
    }

    public IResult Update(AdditionalService service)
    {
        _serviceDal.Update(service);
        return new SuccessResult("Service updated");
    }
}
