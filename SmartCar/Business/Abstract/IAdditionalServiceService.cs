using Core.Utilities.Results;
using Entities.Concrete;
using System.Collections.Generic;

namespace Business.Abstract;

public interface IAdditionalServiceService
{
    IDataResult<List<AdditionalService>> GetAll();
    IDataResult<AdditionalService> GetById(int id);
    IResult Add(AdditionalService service);
    IResult Update(AdditionalService service);
    IResult Delete(AdditionalService service);
}
