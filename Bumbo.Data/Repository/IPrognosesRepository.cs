using System;
using System.Collections.Generic;
using Bumbo.Data.Models;

namespace Bumbo.Data.Repository
{
    public interface IPrognosesRepository
    {
        List<Prognoses> GetAll(DateTime start, DateTime end);

        Prognoses Get(DateTime date, int branchId);

        Prognoses Create(Prognoses prog);

        Prognoses Update(Prognoses prog);

        bool Delete(DateTime date, int branchId);
    }
}
