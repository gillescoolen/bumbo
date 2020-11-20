using System;
using System.Collections.Generic;
using Bumbo.Data.Models;

namespace Bumbo.Data.Repository
{
    public interface IPrognosesRepository
    {
        List<Prognoses> GetAll();

        Prognoses Get(DateTime date);

        Prognoses Create(Prognoses prog);

        Prognoses Update(Prognoses prog);

        bool Delete(DateTime date);
    }
}
