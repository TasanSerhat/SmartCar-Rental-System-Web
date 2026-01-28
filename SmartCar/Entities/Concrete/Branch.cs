using Core.Entities;

namespace Entities.Concrete;

public class Branch : IEntity
{
    public int BranchId { get; set; }
    public string BranchName { get; set; }
    public string City { get; set; }
    public string Address { get; set; }
    public string Phone { get; set; }
}
