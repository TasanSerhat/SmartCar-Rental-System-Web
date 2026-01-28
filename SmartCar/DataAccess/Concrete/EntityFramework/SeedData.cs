using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace DataAccess.Concrete.EntityFramework;

public static class SeedData
{
    public static void Initialize(SmartCarContext context)
    {
        // context.Database.Migrate(); // Migration tool issues
        context.Database.EnsureCreated();

        // 1. Seed Branches
        if (!context.Branches.Any())
        {
            context.Branches.AddRange(
                new Branch { BranchName = "Istanbul Central", City = "Istanbul", Address = "Levent No:1", Phone = "02120000000" },
                new Branch { BranchName = "Ankara Kızılay", City = "Ankara", Address = "Kızılay No:5", Phone = "03120000000" },
                new Branch { BranchName = "Izmir Airport", City = "Izmir", Address = "Adnan Menderes Havalimani", Phone = "02320000000" }
            );
            context.SaveChanges();
        }

        // 2. Seed Colors
        if (!context.Colors.Any())
        {
            context.Colors.AddRange(
                new Color { ColorName = "Red" },
                new Color { ColorName = "Blue" },
                new Color { ColorName = "White" },
                new Color { ColorName = "Black" },
                new Color { ColorName = "Grey" }
            );
            context.SaveChanges();
        }

        // 3. Seed Vehicle Models
        if (!context.VehicleModels.Any())
        {
            context.VehicleModels.AddRange(
                new VehicleModel { Brand = "Tesla", ModelName = "Model S", Year = 2024, TransmissionType = "Automatic", PricePerDay = 3500 },
                new VehicleModel { Brand = "BMW", ModelName = "320i", Year = 2023, TransmissionType = "Automatic", PricePerDay = 2800 },
                new VehicleModel { Brand = "Mercedes", ModelName = "C200", Year = 2023, TransmissionType = "Automatic", PricePerDay = 3000 },
                new VehicleModel { Brand = "Renault", ModelName = "Clio", Year = 2022, TransmissionType = "Manual", PricePerDay = 1200 },
                new VehicleModel { Brand = "Toyota", ModelName = "Corolla Hybrid", Year = 2024, TransmissionType = "Automatic", PricePerDay = 1800 }
            );
            context.SaveChanges();
        }

        // 4. Seed Vehicles
        if (!context.Vehicles.Any())
        {
            var istBranch = context.Branches.First(b => b.City == "Istanbul").BranchId;
            var ankBranch = context.Branches.First(b => b.City == "Ankara").BranchId;
            var izmBranch = context.Branches.First(b => b.City == "Izmir").BranchId;

            var teslaModel = context.VehicleModels.First(m => m.ModelName == "Model S").ModelId;
            var bmwModel = context.VehicleModels.First(m => m.ModelName == "320i").ModelId;
            var c200Model = context.VehicleModels.First(m => m.ModelName == "C200").ModelId;
            var clioModel = context.VehicleModels.First(m => m.ModelName == "Clio").ModelId;

            // Fetch Color IDs (Keeping Colors table as agreed)
            int red = context.Colors.First(c => c.ColorName == "Red").ColorId;
            int black = context.Colors.First(c => c.ColorName == "Black").ColorId;
            int white = context.Colors.First(c => c.ColorName == "White").ColorId;
            int blue = context.Colors.First(c => c.ColorName == "Blue").ColorId;

            context.Vehicles.AddRange(
                new Vehicle { PlateNo = "34 TES 01", ModelId = teslaModel, BranchId = istBranch, ColorId = red,      Description="Luxury Electric", Kilometer = 5000, FuelType = "Electric", Status = "Available" },
                new Vehicle { PlateNo = "34 BMW 99", ModelId = bmwModel,   BranchId = istBranch, ColorId = black,    Description="Sport Sedan",     Kilometer = 15000, FuelType = "Gasoline", Status = "Available" },
                new Vehicle { PlateNo = "06 MER 33", ModelId = c200Model,  BranchId = ankBranch, ColorId = white,    Description="Comfort Class",   Kilometer = 8000, FuelType = "Diesel", Status = "Rented" },
                new Vehicle { PlateNo = "35 REN 55", ModelId = clioModel,  BranchId = izmBranch, ColorId = blue,     Description="Economic Choice", Kilometer = 45000, FuelType = "Gasoline", Status = "Maintenance" },
                new Vehicle { PlateNo = "34 TES 02", ModelId = teslaModel, BranchId = istBranch, ColorId = black,    Description="Luxury Electric", Kilometer = 2000, FuelType = "Electric", Status = "Available" }
            );
            context.SaveChanges();
        }

        // 5. Seed Customers
        if (!context.Customers.Any())
        {
            context.Customers.AddRange(
                new Customer { FirstName = "Ahmet", LastName = "Yilmaz", Email = "ahmet@example.com", PasswordHash = new byte[0], PasswordSalt = new byte[0], Status = true, LicenseNo = "B123456", Phone = "5551112233", Address = "Istanbul" },
                new Customer { FirstName = "Ayse", LastName = "Demir", Email = "ayse@example.com", PasswordHash = new byte[0], PasswordSalt = new byte[0], Status = true, LicenseNo = "B654321", Phone = "5559998877", Address = "Ankara" }
            );
            context.SaveChanges();
        }
    }
}
