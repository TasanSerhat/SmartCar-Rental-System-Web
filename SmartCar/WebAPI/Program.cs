using DataAccess.Concrete.EntityFramework;
using Entities.Concrete;
using Core.Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Core.Utilities.Security.JWT;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// Enable CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

// Services DI
builder.Services.AddScoped<Business.Abstract.IVehicleService, Business.Concrete.VehicleManager>();
builder.Services.AddScoped<DataAccess.Abstract.IVehicleDal, DataAccess.Concrete.EntityFramework.EfVehicleDal>();
builder.Services.AddScoped<Business.Abstract.ICustomerService, Business.Concrete.CustomerManager>();
builder.Services.AddScoped<DataAccess.Abstract.ICustomerDal, DataAccess.Concrete.EntityFramework.EfCustomerDal>();
builder.Services.AddScoped<Business.Abstract.IReservationService, Business.Concrete.ReservationManager>();
builder.Services.AddScoped<DataAccess.Abstract.IReservationDal, DataAccess.Concrete.EntityFramework.EfReservationDal>();
builder.Services.AddScoped<DataAccess.Abstract.IVehicleImageDal, DataAccess.Concrete.EntityFramework.EfVehicleImageDal>();

// Auth DI
builder.Services.AddScoped<Business.Abstract.IUserService, Business.Concrete.UserManager>();
builder.Services.AddScoped<DataAccess.Abstract.IUserDal, DataAccess.Concrete.EntityFramework.EfUserDal>();
builder.Services.AddScoped<Business.Abstract.IAuthService, Business.Concrete.AuthManager>();
builder.Services.AddScoped<Core.Utilities.Security.JWT.ITokenHelper, Core.Utilities.Security.JWT.JwtHelper>();
builder.Services.AddScoped<Business.Abstract.IVehicleImageService, Business.Concrete.VehicleImageManager>();
builder.Services.AddScoped<Business.Abstract.IRentalService, Business.Concrete.RentalManager>();
builder.Services.AddScoped<DataAccess.Abstract.IRentalDal, DataAccess.Concrete.EntityFramework.EfRentalDal>();
builder.Services.AddScoped<Core.Utilities.Helpers.IFileHelper, Core.Utilities.Helpers.FileHelper>();

// Master Data DI
builder.Services.AddScoped<Business.Abstract.IColorService, Business.Concrete.ColorManager>();
builder.Services.AddScoped<DataAccess.Abstract.IColorDal, DataAccess.Concrete.EntityFramework.EfColorDal>();

builder.Services.AddScoped<Business.Abstract.IBranchService, Business.Concrete.BranchManager>();
builder.Services.AddScoped<DataAccess.Abstract.IBranchDal, DataAccess.Concrete.EntityFramework.EfBranchDal>();

builder.Services.AddScoped<Business.Abstract.IVehicleModelService, Business.Concrete.VehicleModelManager>();
builder.Services.AddScoped<DataAccess.Abstract.IVehicleModelDal, DataAccess.Concrete.EntityFramework.EfVehicleModelDal>();

// Employee & Additional Service DI
builder.Services.AddScoped<Business.Abstract.IEmployeeService, Business.Concrete.EmployeeManager>();
builder.Services.AddScoped<DataAccess.Abstract.IEmployeeDal, DataAccess.Concrete.EntityFramework.EfEmployeeDal>();

builder.Services.AddScoped<Business.Abstract.IAdditionalServiceService, Business.Concrete.AdditionalServiceManager>();
builder.Services.AddScoped<DataAccess.Abstract.IAdditionalServiceDal, DataAccess.Concrete.EntityFramework.EfAdditionalServiceDal>();

// JWT Configuration
var tokenOptions = builder.Configuration.GetSection("TokenOptions").Get<Core.Utilities.Security.JWT.TokenOptions>();

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = tokenOptions.Issuer,
            ValidAudience = tokenOptions.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(tokenOptions.SecurityKey))
        };
    });

// Register DbContext
builder.Services.AddDbContext<SmartCarContext>(options =>
    options.UseNpgsql("Host=localhost;Port=5432;Database=SmartCarDb;Username=postgres;Password=1955"));

var app = builder.Build();

// Run Seeds
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<SmartCarContext>();
        DataAccess.Concrete.EntityFramework.SeedData.Initialize(context);
    }
    catch (Exception ex)
    {
        Console.WriteLine("An error occurred seeding the DB: " + ex.Message);
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AllowAll");

app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Clean root endpoint for user verification
app.MapGet("/", () => "SmartCar WebAPI is running! Go to /api/Vehicles/getall to see data.");

app.Run();
