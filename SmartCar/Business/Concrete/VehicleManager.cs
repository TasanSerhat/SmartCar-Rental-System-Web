using Business.Abstract;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using System.Collections.Generic;

namespace Business.Concrete;

public class VehicleManager : IVehicleService
{
    IVehicleDal _vehicleDal;
    IVehicleImageService _vehicleImageService;
    IRentalDal _rentalDal; 
    IReservationDal _reservationDal; // Injected

    public VehicleManager(IVehicleDal vehicleDal, IVehicleImageService vehicleImageService, IRentalDal rentalDal, IReservationDal reservationDal)
    {
        _vehicleDal = vehicleDal;
        _vehicleImageService = vehicleImageService;
        _rentalDal = rentalDal;
        _reservationDal = reservationDal;
    }

    public IResult Add(Vehicle vehicle)
    {
        // Business codes here
        _vehicleDal.Add(vehicle);
        return new SuccessResult("Vehicle added successfully");
    }

    public IResult Delete(Vehicle vehicle)
    {
        try 
        {
            // 1. Delete Images
            var imagesResult = _vehicleImageService.GetByVehicleId(vehicle.VehicleId);
            if (imagesResult.Success && imagesResult.Data != null)
            {
                foreach (var image in imagesResult.Data)
                {
                    if (image.ImagePath != "/images/default.jpg") 
                    {
                        _vehicleImageService.Delete(image);
                    }
                }
            }

            // 2. Delete Dependencies (Reservations -> Rentals)
            // Get all reservations for this vehicle
            var reservations = _reservationDal.GetAll(r => r.VehicleId == vehicle.VehicleId);
            
            foreach (var reservation in reservations)
            {
                // Find rental for this reservation
                var rental = _rentalDal.Get(r => r.ReservationId == reservation.ReservationId);
                if (rental != null)
                {
                     _rentalDal.Delete(rental);
                }

                // Delete Reservation
                _reservationDal.Delete(reservation);
            }

            // 3. Delete Vehicle
            _vehicleDal.Delete(vehicle);
            return new SuccessResult("Vehicle deleted successfully");
        }
        catch (System.Exception ex)
        {
            return new ErrorResult($"Delete failed: {ex.Message} " + (ex.InnerException != null ? ex.InnerException.Message : ""));
        }
    }

    public IDataResult<List<Vehicle>> GetAll()
    {
        // Use GetListWithDetails to populate Model data (Price, Year) and Images
        return new SuccessDataResult<List<Vehicle>>(_vehicleDal.GetListWithDetails());
    }

    public IDataResult<Vehicle> GetById(int vehicleId)
    {
        return new SuccessDataResult<Vehicle>(_vehicleDal.Get(v => v.VehicleId == vehicleId));
    }

    public IDataResult<Vehicle> GetWithDetails(int vehicleId)
    {
        return new SuccessDataResult<Vehicle>(_vehicleDal.GetWithDetails(vehicleId));
    }

    public IResult Update(Vehicle vehicle)
    {
        _vehicleDal.Update(vehicle);
        return new SuccessResult("Vehicle updated successfully");
    }
}
