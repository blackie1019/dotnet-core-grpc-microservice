using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MockSite.Core.DTOs;
using MockSite.Core.Entities;
using MockSite.Core.Interfaces;

namespace MockSite.Core.Services
{
    public class CurrencyService : ICurrencyService
    {
        private readonly ICurrencyRepository _currencyRepository;

        public CurrencyService(ICurrencyRepository currencyRepository)
        {
            _currencyRepository = currencyRepository;
        }

        public async Task Modify(CurrencyDto currencyDto)
        {
            await _currencyRepository.Modify(currencyDto);
        }

        public async Task Delete(string code)
        {
            await _currencyRepository.Delete(code);
        }

        public async Task<CurrencyEntity> GetByCode(string code)
        {
            var currencies = await _currencyRepository.GetCurrencyAll();

            return currencies.FirstOrDefault(c => c.CurrencyCode == code);
        }

        public async Task<IEnumerable<CurrencyEntity>> GetCurrencyAll()
        {
            var currencies = await _currencyRepository.GetCurrencyAll();

            return currencies;
        }

        public async Task<IEnumerable<CurrencyEntity>> GetTtlCurrencies()
        {
            var currencies = await _currencyRepository.GetTtlCurrencies();

            return currencies;
        }
    }
}