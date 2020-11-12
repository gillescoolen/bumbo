using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Bumbo.Domain.Data;

namespace Bumbo.Domain.Models
{
  public static class SeedData
  {
    public static void Initialize(IServiceProvider serviceProvider)
    {
      using var context = new DatabaseContext(
          serviceProvider.GetRequiredService<DbContextOptions<DatabaseContext>>()
          );

    }
  }
}