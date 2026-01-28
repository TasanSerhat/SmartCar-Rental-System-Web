using Core.Utilities.Results;
using Entities.Concrete;
using Entities.DTOs; // Added
using System.Collections.Generic;

namespace Business.Abstract;

public interface ICustomerService
{
    IDataResult<List<CustomerDetailDto>> GetAll();
    IDataResult<Customer> GetById(int customerId);
    IResult Add(Customer customer);
    IResult Update(Customer customer);
    IResult Delete(Customer customer);
}
