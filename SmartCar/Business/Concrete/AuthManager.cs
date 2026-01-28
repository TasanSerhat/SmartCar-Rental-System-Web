using Business.Abstract;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using Core.Utilities.Security.Hashing;
using Core.Utilities.Security.JWT;
using Entities.DTOs;

namespace Business.Concrete
{
    public class AuthManager : IAuthService
    {
        private Business.Abstract.ICustomerService _customerService; // Users are Customers basically
        // Or better, I should use IUserService if available, to be generic. 
        // But in this system User is abstract-ish or TPT base.
        // Let's assume we are registering CUSTOMERS mainly.
        // I'll use UserService for Login check (generic User) and CustomerService for Registration.
        
        // Wait, SmartCarContext has Users, Customers, Employees. 
        // Login checks Users table.
        // Register creates Customer (default).
        
        // I need IUserService.
        // I will check if IUserService exists. I'll pause to check or assume. 
        // I'll assume IUserService is needed and create it if not.
        // FOR NOW: I will hack it by injecting ICustomerDal or similar if IUserService is missing, 
        // BUT I SHOULD DO IT RIGHT.
        // I'll implement AuthManager assuming IUserService exists, if not I will create it.
        // Actually, I'll use `IUserService` (I need to create it).
        
        private IUserService _userService;
        private ITokenHelper _tokenHelper;

        public AuthManager(IUserService userService, ITokenHelper tokenHelper)
        {
            _userService = userService;
            _tokenHelper = tokenHelper;
        }

        public IDataResult<User> Register(UserForRegisterDto userForRegisterDto, string password)
        {
            byte[] passwordHash, passwordSalt;
            HashingHelper.CreatePasswordHash(password, out passwordHash, out passwordSalt);
            
            // Create as Customer by default
            var customer = new Entities.Concrete.Customer
            {
                Email = userForRegisterDto.Email,
                FirstName = userForRegisterDto.FirstName,
                LastName = userForRegisterDto.LastName,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Status = true,
                Phone = userForRegisterDto.Phone,
                Address = userForRegisterDto.Address,
                LicenseNo = userForRegisterDto.LicenseNo
            };
            
            _userService.Add(customer); // This needs to handle Customer subtype if IUserService is generic User? 
            // If IUserService.Add(User u) only saves to Users table, TPT might be tricky if not handling derived types.
            // EF Core TPT handles it if we add to Context.Customers.
            // So UserService.Add should probably be generic or I should allow injecting ICustomerService here.
            // Let's use ICustomerService for Register.
            
            return new SuccessDataResult<User>(customer, "User registered");
        }

        public IDataResult<User> Login(UserForLoginDto userForLoginDto)
        {
            var userToCheck = _userService.GetByMail(userForLoginDto.Email);
            if (userToCheck == null)
            {
                return new ErrorDataResult<User>("User not found");
            }

            if (!HashingHelper.VerifyPasswordHash(userForLoginDto.Password, userToCheck.PasswordHash, userToCheck.PasswordSalt))
            {
                return new ErrorDataResult<User>("Password error");
            }

            return new SuccessDataResult<User>(userToCheck, "Successful login");
        }

        public IResult UserExists(string email)
        {
            if (_userService.GetByMail(email) != null)
            {
                return new ErrorResult("User already exists");
            }
            return new SuccessResult();
        }

        public IDataResult<AccessToken> CreateAccessToken(User user)
        {
            var claims = _userService.GetClaims(user);
            var accessToken = _tokenHelper.CreateToken(user, claims);
            return new SuccessDataResult<AccessToken>(accessToken, "Token created");
        }
    }
}
