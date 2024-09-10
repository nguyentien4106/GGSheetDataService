using CleanArchitecture.Core.Services;
using CleanArchitecture.Infrastructure.Data;
using DataService.Application.Repositories;
using DataService.Core.Entities;
using DataWorkerService.Helper;
using DataWorkerService.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataService.Application.Services
{
    public class AttendanceService : GenericRepository<Attendance>, IAttendanceService
    {
        IServiceLocator _serviceLocator;
        public AttendanceService(AppDbContext context, IServiceLocator locator) : base(context)
        {
            _serviceLocator = locator;
        }

    }
}
