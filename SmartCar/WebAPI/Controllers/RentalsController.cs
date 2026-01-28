using Business.Abstract;
using Entities.Concrete;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RentalsController : ControllerBase
    {
        IRentalService _rentalService;
        IReservationService _reservationService;
        IVehicleService _vehicleService;

        public RentalsController(IRentalService rentalService, IReservationService reservationService, IVehicleService vehicleService)
        {
            _rentalService = rentalService;
            _reservationService = reservationService;
            _vehicleService = vehicleService;
        }

        [HttpGet("getall")]
        public IActionResult GetAll()
        {
            var result = _rentalService.GetAll();
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("add")] // Start Rental
        public IActionResult Add(Rental rental)
        {
            // 1. Add Rental
            var result = _rentalService.Add(rental);
            if (!result.Success) return BadRequest(result.Message);

            // 2. Update Reservation Status -> Rented
            var reservationRes = _reservationService.GetById(rental.ReservationId);
            if (reservationRes.Success && reservationRes.Data != null)
            {
                var reservation = reservationRes.Data;
                reservation.Status = "Rented";
                _reservationService.Update(reservation);

                // 3. Update Vehicle Status -> Rented
                var vehicleRes = _vehicleService.GetById(reservation.VehicleId);
                if (vehicleRes.Success && vehicleRes.Data != null)
                {
                    var vehicle = vehicleRes.Data;
                    vehicle.Status = "Rented";
                    _vehicleService.Update(vehicle);
                }
            }

            return Ok(result);
        }

        [HttpPost("endrental")]
        public IActionResult EndRental([FromQuery] int rentalId)
        {
            var rentalRes = _rentalService.GetById(rentalId);
            if (!rentalRes.Success || rentalRes.Data == null) return BadRequest("Rental not found");

            var rental = rentalRes.Data;
            
            // 1. Update Reservation -> Completed
            var reservationRes = _reservationService.GetById(rental.ReservationId);
            if (reservationRes.Success && reservationRes.Data != null)
            {
                var reservation = reservationRes.Data;
                reservation.Status = "Completed";
                _reservationService.Update(reservation);

                // 2. Update Vehicle -> Available
                var vehicleRes = _vehicleService.GetById(reservation.VehicleId);
                if (vehicleRes.Success && vehicleRes.Data != null)
                {
                    var vehicle = vehicleRes.Data;
                    vehicle.Status = "Available";
                    _vehicleService.Update(vehicle);
                }
            }

            // We don't necessarily delete the rental, we keep it as history. 
            // Or maybe we update a "ReturnDate" if we added it. 
            // For now, the Reservation Status "Completed" signifies it's done.
            return Ok(new { message = "Rental ended successfully." });
        }
    }
}
