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
        public List<Prognoses> GetAll(DateTime start, DateTime end, int branchId)
        {
            using (var ctx = new ContextFactory().CreateDbContext(null))
            {
                //return ctx.Prognoses.ToList();
                return ctx.Prognoses.Where(n => n.Date >= start).Where(n => n.Date <= end).Where(n => n.BranchId == branchId).ToList();
            }
        }

        public Prognoses Get(DateTime date, int branchId)
        {
            using (var ctx = new ContextFactory().CreateDbContext(null))
            {
                return ctx.Prognoses.Where(n => n.Date == date.Date).Where(n => n.BranchId == branchId).FirstOrDefault();
            }
        }

        public bool Create(Prognoses prog)
        {
            using (var ctx = new ContextFactory().CreateDbContext(null))
            {
                ctx.Prognoses.Add(prog);
                return ctx.SaveChanges() > 0;
            }
        }

        public bool Update(Prognoses prog)
        {
            using (var ctx = new ContextFactory().CreateDbContext(null))
            {
                ctx.Prognoses.Update(prog);
                return ctx.SaveChanges() > 0;
            }
        }

        public bool Delete(DateTime date, int branchId)
        {
            using (var ctx = new ContextFactory().CreateDbContext(null))
            {
                var toRemove = ctx.Prognoses.Find(date.Date, branchId);
                if (toRemove != null)
                {
                    ctx.Prognoses.Remove(toRemove);
                    ctx.SaveChanges();
                    return true;
                }
                return false;
            }
        }
    }
}
