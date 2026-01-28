using Business.Abstract;
using Core.Utilities.Security.Hashing;
using Core.Utilities.Results; 
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.DTOs; // Added
using System.Collections.Generic;
using System.Linq; // Added

namespace Business.Concrete;

public class CustomerManager : ICustomerService
{
    ICustomerDal _customerDal;

    public CustomerManager(ICustomerDal customerDal)
    {
        _customerDal = customerDal;
    }

    public IResult Add(Customer customer)
    {
        // Business rules
        if (customer.PasswordHash == null || customer.PasswordHash.Length == 0)
        {
             HashingHelper.CreatePasswordHash("123456", out byte[] passwordHash, out byte[] passwordSalt);
             customer.PasswordHash = passwordHash;
             customer.PasswordSalt = passwordSalt;
        }

        _customerDal.Add(customer);
        return new SuccessResult("Customer added successfully");
    }

    public IResult Delete(Customer customer)
    {
        _customerDal.Delete(customer);
        return new SuccessResult("Customer deleted successfully");
    }

    public IDataResult<List<CustomerDetailDto>> GetAll()
    {
        var customers = _customerDal.GetAll();
        var dtos = customers.Select(c => new CustomerDetailDto
        {
            Id = c.Id,
            FirstName = c.FirstName,
            LastName = c.LastName,
            Email = c.Email,
            Address = c.Address,
            Phone = c.Phone,
            LicenseNo = c.LicenseNo
        }).ToList();

        return new SuccessDataResult<List<CustomerDetailDto>>(dtos);
    }

    public IDataResult<Customer> GetById(int customerId)
    {
        return new SuccessDataResult<Customer>(_customerDal.Get(c => c.Id == customerId));
    }

    public IResult Update(Customer customer)
    {
        var result = GetById(customer.Id);
        if (result.Data != null)
        {
            var existingCustomer = result.Data;
            customer.PasswordHash = existingCustomer.PasswordHash;
            customer.PasswordSalt = existingCustomer.PasswordSalt;
        }
        _customerDal.Update(customer);
        return new SuccessResult("Customer updated successfully");
    }
}
