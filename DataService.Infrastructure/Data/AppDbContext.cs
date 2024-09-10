using CleanArchitecture.Core.Entities;
using DataService.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace CleanArchitecture.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext()
    {
        
    }
    public AppDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Test> Tests { get; set; }

    public DbSet<Device> Devices { get; set; }

    public DbSet<Attendance> Attendances { get; set; }

    public DbSet<Employee> Employees { get; set; }

    public DbSet<Sheet> Sheets { get; set; }    

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Sheet>().HasOne(t => t.Device).WithMany(t => t.Sheets).OnDelete(DeleteBehavior.ClientCascade);
        base.OnModelCreating(builder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options) =>
        options.UseNpgsql("Server=localhost;Port=5432;Database=AttDB;User Id=postgres;Password=Ti100600@;");
}
