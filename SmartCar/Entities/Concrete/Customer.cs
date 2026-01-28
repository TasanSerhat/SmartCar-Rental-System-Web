using Core.Entities;
using Core.Entities.Concrete; // Added

namespace Entities.Concrete;

public class Customer : User // User is now ambiguous if I don't specify or remove old User
// But old User is deleted. So default User refers to Core...User if I add using.
{
    // Id is inherited
    public string Address { get; set; }
    public string Phone { get; set; }
    public string LicenseNo { get; set; }
}
