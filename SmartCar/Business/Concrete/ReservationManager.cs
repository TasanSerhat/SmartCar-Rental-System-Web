using Business.Abstract;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Business.Concrete;

public class ReservationManager : IReservationService
{
    private readonly IReservationDal _reservationDal;
    private readonly IVehicleService _vehicleService;

    public ReservationManager(IReservationDal reservationDal, IVehicleService vehicleService)
    {
        _reservationDal = reservationDal;
        _vehicleService = vehicleService;
    }

    public IResult Add(Reservation reservation)
    {
        try 
        {
            // Force UTC for Postgres
            if (reservation.StartDate.Kind == DateTimeKind.Unspecified)
                reservation.StartDate = DateTime.SpecifyKind(reservation.StartDate, DateTimeKind.Utc);
            
            if (reservation.EndDate.Kind == DateTimeKind.Unspecified)
                reservation.EndDate = DateTime.SpecifyKind(reservation.EndDate, DateTimeKind.Utc);

            if (reservation.ReservationDate.Kind == DateTimeKind.Unspecified)
                reservation.ReservationDate = DateTime.SpecifyKind(reservation.ReservationDate, DateTimeKind.Utc);
            else
                reservation.ReservationDate = reservation.ReservationDate.ToUniversalTime();

            // Validation: Dates
            if (reservation.StartDate >= reservation.EndDate)
            {
                return new ErrorResult("Start date must be before end date.");
            }

            if (reservation.StartDate < DateTime.UtcNow.Date)
            {
                return new ErrorResult("Reservation start date cannot be in the past.");
            }

            // Get Vehicle for Price Calculation (with Model/Price details)
            var vehicleResult = _vehicleService.GetWithDetails(reservation.VehicleId);
            if (!vehicleResult.Success || vehicleResult.Data == null)
            {
                return new ErrorResult("Vehicle not found.");
            }
            var vehicle = vehicleResult.Data;

            // Calculate Price
            var days = (reservation.EndDate - reservation.StartDate).Days;
            if (days <= 0) days = 1; // Minimum 1 day

            // Update to use Model.PricePerDay
            reservation.TotalPrice = days * vehicle.Model.PricePerDay;

            // Check if vehicle is available
            var overlappingReservation = _reservationDal.GetAll(r => 
                r.VehicleId == reservation.VehicleId && 
                (
                    (reservation.StartDate >= r.StartDate && reservation.StartDate <= r.EndDate) ||
                    (reservation.EndDate >= r.StartDate && reservation.EndDate <= r.EndDate) ||
                    (reservation.StartDate <= r.StartDate && reservation.EndDate >= r.EndDate)
                )
            ).Any();

            if (overlappingReservation)
            {
                return new ErrorResult("This vehicle is already reserved for the selected dates.");
            }

            _reservationDal.Add(reservation);
            return new SuccessResult($"Reservation created successfully. Total Price: ${reservation.TotalPrice}");
        }
        catch (Exception ex)
        {
            return new ErrorResult($"Internal Error: {ex.Message} - {ex.InnerException?.Message}");
        }
    }


    public IResult Delete(Reservation reservation)
    {
        _reservationDal.Delete(reservation);
        return new SuccessResult("Reservation deleted.");
    }

    public IDataResult<List<ReservationDetailDto>> GetReservationDetails()
    {
        return new SuccessDataResult<List<ReservationDetailDto>>(_reservationDal.GetReservationDetails());
    }

    public IDataResult<List<Reservation>> GetAll()
    {
        return new SuccessDataResult<List<Reservation>>(_reservationDal.GetAll());
    }

    public IDataResult<Reservation> GetById(int id)
    {
        return new SuccessDataResult<Reservation>(_reservationDal.Get(r => r.ReservationId == id));
    }

    public IResult Update(Reservation reservation)
    {
        _reservationDal.Update(reservation);
        return new SuccessResult("Reservation updated.");
    }
}
