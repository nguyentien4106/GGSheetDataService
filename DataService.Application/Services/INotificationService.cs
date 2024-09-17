using DataService.Core.Contracts;
using DataService.Infrastructure.Entities;


namespace DataService.Application.Services
{
    public interface INotificationService : IGenericRepository<Notification>
    {
    }
}
