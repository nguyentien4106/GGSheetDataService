using DataService.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace DataService.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext()
    {
        
    }
    public AppDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Device> Devices { get; set; }

    public DbSet<Attendance> Attendances { get; set; }

    public DbSet<Employee> Employees { get; set; }

    public DbSet<Sheet> Sheets { get; set; }    
    public DbSet<Test> Tests { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Sheet>().HasOne(t => t.Device).WithMany(t => t.Sheets).HasForeignKey(item => item.DeviceId).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<Attendance>().HasOne(t => t.Device).WithMany(t => t.Attendances).HasForeignKey(i => i.DeviceId).OnDelete(DeleteBehavior.Restrict);
        base.OnModelCreating(builder);
    }

    //protected override void OnConfiguring(DbContextOptionsBuilder options) =>
    //    options.UseNpgsql("Server=localhost;Port=5432;Database=AttDB;User Id=postgres;Password=Ti100600@;");
}
