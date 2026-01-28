using Core.DataAccess.EntityFramework;
using DataAccess.Abstract;
using Entities.Concrete;
using DataAccess.Concrete.EntityFramework;

namespace DataAccess.Concrete.EntityFramework;

public class EfCustomerDal : EfEntityRepositoryBase<Customer, SmartCarContext>, ICustomerDal
{
}
