using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Bumbo.Data
{
    class ContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        private IConfiguration Configuration => new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json")
           .Build();

        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            builder.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));

            return new ApplicationDbContext(builder.Options);
        }
    }
}
