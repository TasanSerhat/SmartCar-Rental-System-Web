using Business.Abstract;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using System.Collections.Generic;

namespace Business.Concrete;

public class EmployeeManager : IEmployeeService
{
    IEmployeeDal _employeeDal;

    public EmployeeManager(IEmployeeDal employeeDal)
    {
        _employeeDal = employeeDal;
    }

    public IResult Add(Employee employee)
    {
        _employeeDal.Add(employee);
        return new SuccessResult("Employee added");
    }

    public IResult Delete(Employee employee)
    {
        _employeeDal.Delete(employee);
        return new SuccessResult("Employee deleted");
    }

    public IDataResult<List<Employee>> GetAll()
    {
        return new SuccessDataResult<List<Employee>>(_employeeDal.GetAll());
    }

    public IDataResult<Employee> GetById(int id)
    {
        return new SuccessDataResult<Employee>(_employeeDal.Get(e => e.Id == id));
    }

    public IResult Update(Employee employee)
    {
        _employeeDal.Update(employee);
        return new SuccessResult("Employee updated");
    }
}
