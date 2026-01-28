using Core.DataAccess.EntityFramework;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework;
using Entities.Concrete;

namespace DataAccess.Concrete.EntityFramework;

public class EfEmployeeDal : EfEntityRepositoryBase<Employee, SmartCarContext>, IEmployeeDal
{
}
