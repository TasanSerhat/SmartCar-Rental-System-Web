using Core.DataAccess.EntityFramework;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework;
using Entities.Concrete;
using Entities.DTOs;
using System.Collections.Generic;
using System.Linq;

namespace DataAccess.Concrete.EntityFramework;

public class EfReservationDal : EfEntityRepositoryBase<Reservation, SmartCarContext>, IReservationDal
{
    public List<ReservationDetailDto> GetReservationDetails()
    {
        using (SmartCarContext context = new SmartCarContext())
        {
            var result = from r in context.Reservations
                         join v in context.Vehicles on r.VehicleId equals v.VehicleId into vGroup
                         from v in vGroup.DefaultIfEmpty()
                         join c in context.Customers on r.CustomerId equals c.Id into cGroup
                         from c in cGroup.DefaultIfEmpty()
                         select new ReservationDetailDto
                         {
                             ReservationId = r.ReservationId,
                             CustomerName = c != null ? c.FirstName + " " + c.LastName : "Unknown Customer",
                             VehiclePlate = v != null ? v.PlateNo : "Unknown Vehicle",
                             StartDate = r.StartDate,
                             EndDate = r.EndDate,
                             TotalPrice = r.TotalPrice,
                             Status = r.Status
                         };
            return result.ToList();
        }
    }
}
