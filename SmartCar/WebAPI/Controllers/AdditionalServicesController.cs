using Business.Abstract;
using Entities.Concrete;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AdditionalServicesController : ControllerBase
{
    IAdditionalServiceService _serviceService;

    public AdditionalServicesController(IAdditionalServiceService serviceService)
    {
        _serviceService = serviceService;
    }

    [HttpGet("getall")]
    public IActionResult GetAll()
    {
        var result = _serviceService.GetAll();
        if (result.Success) return Ok(result);
        return BadRequest(result);
    }

    [HttpPost("add")]
    public IActionResult Add(AdditionalService service)
    {
        var result = _serviceService.Add(service);
        if (result.Success) return Ok(result);
        return BadRequest(result);
    }

    [HttpPost("update")]
    public IActionResult Update(AdditionalService service)
    {
        var result = _serviceService.Update(service);
        if (result.Success) return Ok(result);
        return BadRequest(result);
    }

    [HttpPost("delete")]
    public IActionResult Delete(AdditionalService service)
    {
        var result = _serviceService.Delete(service);
        if (result.Success) return Ok(result);
        return BadRequest(result);
    }
}
