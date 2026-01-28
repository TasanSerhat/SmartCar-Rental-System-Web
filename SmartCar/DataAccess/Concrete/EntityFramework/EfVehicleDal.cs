using Core.DataAccess.EntityFramework;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework;
using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;

namespace DataAccess.Concrete.EntityFramework;

public class EfVehicleDal : EfEntityRepositoryBase<Vehicle, SmartCarContext>, IVehicleDal
{
    public Vehicle GetWithDetails(int id)
    {
        using (SmartCarContext context = new SmartCarContext())
        {
            return context.Vehicles
                .Include(v => v.Model)
                .Include(v => v.Color)
                .Include(v => v.Branch)
                .Include(v => v.VehicleImages) // Uncommented
                .SingleOrDefault(v => v.VehicleId == id);
        }
    }

    public List<Vehicle> GetListWithDetails(Expression<Func<Vehicle, bool>> filter = null)
    {
        using (SmartCarContext context = new SmartCarContext())
        {
            return filter == null
                ? context.Vehicles
                    .Include(v => v.Model)
                    .Include(v => v.Color)
                    .Include(v => v.Branch)
                    .Include(v => v.VehicleImages)
                    .ToList()
                : context.Vehicles
                    .Include(v => v.Model)
                    .Include(v => v.Color)
                    .Include(v => v.Branch)
                    .Include(v => v.VehicleImages)
                    .Where(filter)
                    .ToList();
        }
    }
}
