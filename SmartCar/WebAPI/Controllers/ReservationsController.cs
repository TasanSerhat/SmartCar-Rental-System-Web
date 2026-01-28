using Business.Abstract;
using Entities.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationService _reservationService;

        public ReservationsController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        [HttpGet("getall")]
        public IActionResult GetAll()
        {
            var result = _reservationService.GetAll();
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
        }

        [HttpGet("getdetails")]
        public IActionResult GetReservationDetails()
        {
            var result = _reservationService.GetReservationDetails();
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
        }

        [HttpPost("add")]
        [Authorize]
        public IActionResult Add(Reservation reservation)
        {
            // Extract User ID from Token Claims
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdString, out int userId))
            {
                reservation.CustomerId = userId;
            }
            // For now, if claim is missing (or fails parse), we might allow manual customerId if 0?
            // But strict mode:
            else if (reservation.CustomerId == 0)
            {
                // If claim missing and sent 0, failure.
                // But during dev/testing maybe we allow passthrough? 
                // No, let's enforce claim if available. 
                // If parsing failed (e.g. null), maybe return Unauthorized?
                // But [Authorize] ensures we have a user.
                // If NameIdentifier is missing, something is wrong with Token.
                return Unauthorized("User ID claim not found.");
            }
            // If parsing failed but customerId was sent (e.g. admin adding for someone?), we could allow.
            // But strict requirement: "User rents for themselves".
            
            var result = _reservationService.Add(reservation);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        [HttpPost("update")]
        [Authorize] 
        // Admin or User? Let's leave it Authorize (Any Auth User) for now, or refine later.
        public IActionResult Update(Reservation reservation)
        {
            var result = _reservationService.Update(reservation);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }

        [HttpPost("delete")]
        [Authorize] 
        public IActionResult Delete([FromQuery] int id)
        {
            var reservationResult = _reservationService.GetById(id);
            if (!reservationResult.Success || reservationResult.Data == null) 
                return BadRequest("Reservation not found");

            var result = _reservationService.Delete(reservationResult.Data);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }
    }
}
