using Core.DataAccess.EntityFramework;
using Core.Entities.Concrete;
using DataAccess.Abstract;
using System.Collections.Generic;
using System.Linq;

namespace DataAccess.Concrete.EntityFramework
{
    public class EfUserDal : EfEntityRepositoryBase<User, SmartCarContext>, IUserDal
    {
        public List<OperationClaim> GetClaims(User user)
        {
            using (var context = new SmartCarContext())
            {
                // Assuming OperationClaim and UserOperationClaim tables exist, 
                // BUT User didn't request Role management explicitly yet.
                // I will return empty list for now to satisfy interface or mock basic "User" role.
                // Schema has 'Employee' with roles, but system-wide OperationClaims might not exist in Schema?
                // Checking Schema...
                // Schema has Employee table with 'Role' column.
                // It does NOT have OperationClaim table.
                
                // OK, I will return a "Claim" based on the user type?
                // If user is Employee -> Claim = Employee.Role
                // If user is Customer -> Claim = "Customer"
                
                var claims = new List<OperationClaim>();
                
                // Check if user is Employee
                var employee = context.Employees.SingleOrDefault(e => e.Id == user.Id);
                if (employee != null)
                {
                    claims.Add(new OperationClaim { Name = employee.Role }); // admin, manager, etc.
                }
                else
                {
                    claims.Add(new OperationClaim { Name = "Customer" });
                }
                
                return claims;
            }
        }
    }
}
