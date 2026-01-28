using Core.Entities;
using Core.Entities.Concrete;

namespace Entities.Concrete;

public class Employee : User
{
    public int BranchId { get; set; }
    public string Role { get; set; } // Manager, Technician, Salesperson
    public decimal Salary { get; set; }
    
    public Branch? Branch { get; set; }
}
