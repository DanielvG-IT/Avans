using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Interfaces
{
    public interface IHolidayApiService
    {
        Task<bool> IsHolidayAsync(DateTime date, string countryCode);
    }
}