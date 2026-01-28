using Core.Utilities.Results;
using Entities.Concrete;
using Entities.DTOs;
using System.Collections.Generic;

namespace Business.Abstract;

public interface IReservationService
{
    IDataResult<List<ReservationDetailDto>> GetReservationDetails();
    IDataResult<List<Reservation>> GetAll();
    IDataResult<Reservation> GetById(int id);
    IResult Add(Reservation reservation);
    IResult Update(Reservation reservation);
    IResult Delete(Reservation reservation);
}
