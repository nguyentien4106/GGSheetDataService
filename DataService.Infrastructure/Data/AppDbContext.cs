using DataService.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using System.Reflection.Emit;

namespace DataService.Infrastructure.Data;

public class AppDbContext : DbContext
{
    private readonly IConfiguration _configuration;
    public AppDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public AppDbContext(DbContextOptions options, IConfiguration configuration) : base(options)
    {
        _configuration = configuration;
    }

    public DbSet<Device> Devices { get; set; }

    public DbSet<Attendance> Attendances { get; set; }

    public DbSet<Employee> Employees { get; set; }

    public DbSet<Sheet> Sheets { get; set; }    

    public DbSet<Notification> Notifications { get; set; }

    public DbSet<DeviceEmployee> DeviceEmployees { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Sheet>().HasOne(t => t.Device).WithMany(t => t.Sheets).HasForeignKey(item => item.DeviceId).OnDelete(DeleteBehavior.Cascade);
        //builder.Entity<Attendance>().HasOne(t => t.Device).WithMany(t => t.Attendances).HasForeignKey(i => i.DeviceId).OnDelete(DeleteBehavior.ClientNoAction);
        builder.Entity<Device>().HasMany(item => item.Employees).WithMany(item => item.Devices).UsingEntity<DeviceEmployee>();
        builder.Entity<Device>().HasMany(item => item.Employees).WithMany(item => item.Devices).UsingEntity<DeviceEmployee>();
        base.OnModelCreating(builder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseNpgsql(_configuration.GetConnectionString("DefaultConnection"));
    }
}
