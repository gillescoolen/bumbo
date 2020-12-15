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
        public List<Prognoses> GetAll(DateTime start, DateTime end)
        {
            using (var ctx = new ContextFactory().CreateDbContext(null))
            {
                //return ctx.Prognoses.ToList();
                return ctx.Prognoses.Where(n => n.Date >= start).Where(n => n.Date <= end).ToList();
            }
        }

        public Prognoses Get(DateTime date)
        {
            using (var ctx = new ContextFactory().CreateDbContext(null))
            {
                return ctx.Prognoses.FirstOrDefault(n => n.Date == date.Date);
            }
        }

        public Prognoses Create(Prognoses prog)
        {
            using (var ctx = new ContextFactory().CreateDbContext(null))
            {
                ctx.Prognoses.Add(prog);
                ctx.SaveChanges();
                return prog;
            }
        }

        public Prognoses Update(Prognoses prog)
        {
            using (var ctx = new ContextFactory().CreateDbContext(null))
            {
                ctx.Prognoses.Update(prog);
                ctx.SaveChanges();
                return prog;
            }
        }

        public bool Delete(DateTime date)
        {
            using (var ctx = new ContextFactory().CreateDbContext(null))
            {
                var toRemove = ctx.Prognoses.Find(date.Date);
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
