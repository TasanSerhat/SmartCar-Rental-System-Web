using Business.Abstract;
using Entities.Concrete;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EmployeesController : ControllerBase
{
    IEmployeeService _employeeService;

    public EmployeesController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    [HttpGet("getall")]
    public IActionResult GetAll()
    {
        var result = _employeeService.GetAll();
        if (result.Success) return Ok(result);
        return BadRequest(result);
    }

    [HttpPost("add")]
    public IActionResult Add(Employee employee)
    {
        var result = _employeeService.Add(employee);
        if (result.Success) return Ok(result);
        return BadRequest(result);
    }

    [HttpPost("update")]
    public IActionResult Update(Employee employee)
    {
        var result = _employeeService.Update(employee);
        if (result.Success) return Ok(result);
        return BadRequest(result);
    }

    [HttpPost("delete")]
    public IActionResult Delete(Employee employee)
    {
        // Simple delete by object, or could be by ID. 
        // Typically FE sends ID, we fetch then delete, or just ID if DAL supports it.
        // For simplicity reusing standard pattern (object).
        var result = _employeeService.Delete(employee);
        if (result.Success) return Ok(result);
        return BadRequest(result);
    }
}
