using Core.DataAccess;
using Entities.Concrete;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DataAccess.Abstract;

public interface IVehicleDal : IEntityRepository<Vehicle>
{
    Vehicle GetWithDetails(int id);
    List<Vehicle> GetListWithDetails(Expression<Func<Vehicle, bool>> filter = null);
}
