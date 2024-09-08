using CleanArchitecture.Core.Entities;
using DataService.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace CleanArchitecture.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Test> Tests { get; set; }

    public DbSet<Device> Devices { get; set; }

    public DbSet<Attendance> Attendances { get; set; }

    public DbSet<Employee> Employees { get; set; }

    public DbSet<Sheet> Sheets { get; set; }    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        //modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
