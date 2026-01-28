using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace DataAccess.Concrete.EntityFramework;

public class SmartCarContextFactory : IDesignTimeDbContextFactory<SmartCarContext>
{
    public SmartCarContext CreateDbContext(string[] args)
    {
        // Build configuration to read user secrets or appsettings
        // Assuming appsettings is in WebAPI or current execution dir
        // For simplicity in design time, we can try to find it.
        
        var optionsBuilder = new DbContextOptionsBuilder<SmartCarContext>();
        
        // Hardcoded for design-time simplicity or read from env.
        // User provided: Host=localhost;Port=5432;Database=SmartCarDb;Username=postgres;Password=1955
        var connectionString = "Host=localhost;Port=5432;Database=SmartCarDb;Username=postgres;Password=1955";

        optionsBuilder.UseNpgsql(connectionString);

        return new SmartCarContext(optionsBuilder.Options);
    }
}
