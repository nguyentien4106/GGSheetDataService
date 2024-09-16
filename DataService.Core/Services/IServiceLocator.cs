using System;
using Microsoft.Extensions.DependencyInjection;

namespace DataService.Core.Services;

public interface IServiceLocator : IDisposable
{
      IServiceScope CreateScope();
      T Get<T>();
}
