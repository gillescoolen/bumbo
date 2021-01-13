using System;
using System.Collections.Generic;
using Bumbo.Data.Models;

namespace Bumbo.Data.Repository
{
    public interface IPrognosesRepository
    {
        List<Prognoses> GetAll(DateTime start, DateTime end, int branchId);

        Prognoses Get(DateTime date, int branchId);

        bool Create(Prognoses prog);

        Prognoses Update(Prognoses prog);

        bool Delete(DateTime date, int branchId);
    }
}
