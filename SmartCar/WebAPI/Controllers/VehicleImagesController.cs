using Business.Abstract;
using Entities.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController] 
    public class VehicleImagesController : ControllerBase
    {
        IVehicleImageService _vehicleImageService;

        public VehicleImagesController(IVehicleImageService vehicleImageService)
        {
            _vehicleImageService = vehicleImageService;
        }

        [HttpPost("add")]
        public IActionResult Add([FromForm] IFormFile file, [FromForm] int vehicleId)
        {
            var vehicleImage = new VehicleImage { VehicleId = vehicleId };
            var result = _vehicleImageService.Add(file, vehicleImage);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("delete")]
        public IActionResult Delete([FromForm] int id)
        {
            var vehicleImage = _vehicleImageService.GetById(id).Data;
            var result = _vehicleImageService.Delete(vehicleImage);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("update")]
        public IActionResult Update([FromForm] IFormFile file, [FromForm] int id)
        {
            var vehicleImage = _vehicleImageService.GetById(id).Data;
            var result = _vehicleImageService.Update(file, vehicleImage);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpGet("getall")]
        public IActionResult GetAll()
        {
            var result = _vehicleImageService.GetAll();
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpGet("getbyvehicleid")]
        public IActionResult GetByVehicleId(int vehicleId)
        {
            var result = _vehicleImageService.GetByVehicleId(vehicleId);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}
