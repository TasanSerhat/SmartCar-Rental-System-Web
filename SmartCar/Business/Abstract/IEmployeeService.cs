using Core.Utilities.Results;
using Entities.Concrete;
using System.Collections.Generic;

namespace Business.Abstract;

public interface IEmployeeService
{
    IDataResult<List<Employee>> GetAll();
    IDataResult<Employee> GetById(int id);
    IResult Add(Employee employee);
    IResult Update(Employee employee);
    IResult Delete(Employee employee);
}
