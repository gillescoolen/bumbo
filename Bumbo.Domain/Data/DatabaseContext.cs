using System;
using Microsoft.EntityFrameworkCore;
using Bumbo.Domain.Models;

namespace Bumbo.Domain.Data
{
  public class DatabaseContext : DbContext
  {
    public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options)
    {
    }

  }
}
