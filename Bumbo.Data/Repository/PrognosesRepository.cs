using Microsoft.EntityFrameworkCore;
using Bumbo.Data;
using Bumbo.Data.Models;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Bumbo.Data.Repository
{
    public class PrognosesRepository : IPrognosesRepository
    {
        private readonly ApplicationDbContext context;

        public PrognosesRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public List<Prognoses> GetAll(DateTime start, DateTime end, int branchId)
        {
            return context.Prognoses.Where(n => n.Date >= start).Where(n => n.Date <= end).Where(n => n.BranchId == branchId).ToList();
        }

        public Prognoses Get(DateTime date, int branchId)
        {
            return context.Prognoses.Where(n => n.Date == date.Date).FirstOrDefault(n => n.BranchId == branchId);
        }

        public bool Create(Prognoses prog)
        {
            context.Prognoses.Add(prog);
            return context.SaveChanges() > 0;
        }

        public bool Update(Prognoses prog)
        {
            context.Prognoses.Update(prog);
            return context.SaveChanges() > 0;
        }

        public bool Delete(DateTime date, int branchId)
        {
            var toRemove = context.Prognoses.Find(date.Date, branchId);
            if (toRemove != null)
            {
                context.Prognoses.Remove(toRemove);
                context.SaveChanges();
                return true;
            }
            return false;
        }
    }
}
