using System.Collections.Generic;
using System.Threading.Tasks;
using MockSite.Core.DTOs;
using MockSite.Core.Entities;

namespace MockSite.Core.Interfaces
{
    public interface ICurrencyRepository
    {
        Task Modify(CurrencyDto currencyDto);

        Task Delete(string code);

        Task<IEnumerable<CurrencyEntity>> GetCurrencyAll();

        Task<IEnumerable<CurrencyEntity>> GetTtlCurrencies();
    }
}