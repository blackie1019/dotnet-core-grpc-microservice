using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Grpc.Core;
using MockSite.Core.DTOs;
using MockSite.Core.Entities;
using MockSite.Core.Interfaces;
using MockSite.Message;

namespace MockSite.DomainService
{
    public class CurrencyServiceImpl : CurrencyService.CurrencyServiceBase
    {
        private readonly ICurrencyService _currencyService;

        public CurrencyServiceImpl(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        public override async Task<Currencies> GetTtlCurrencies(Empty request, ServerCallContext context)
        {
            try
            {
                var result = new Currencies();

                var currencyEntities = await _currencyService.GetTtlCurrencies();

                result.Value.AddRange(currencyEntities.Select(entity => new Currency
                {
                    CurrencyCode = entity.CurrencyCode,
                    CurrencyRate = entity.CurrencyRate
                }));

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public override async Task<BaseResponse> Modify(Currency request, ServerCallContext context)
        {
            var result = new BaseResponse();

            try
            {
                await _currencyService.Modify(new CurrencyDto(request.CurrencyCode,request.CurrencyRate));
                
                result.Code = ResponseCode.Success;
            }
            catch (Exception)
            {
                result.Code = ResponseCode.GeneralError;
            }

            return result;
        }

        public override async Task<Currencies> GetCurrencyAll(Empty request, ServerCallContext context)
        {
            try
            {
                var result = new Currencies();

                var currencyEntities = await _currencyService.GetCurrencyAll();

                result.Value.AddRange(currencyEntities.Select(entity => new Currency
                {
                    CurrencyCode = entity.CurrencyCode,
                    CurrencyRate = entity.CurrencyRate
                }));

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public override async Task<Currency> Get(QueryCurrencyMessage request, ServerCallContext context)
        {
            var currency = new Currency();
            try
            {
                var currencyEntity = await _currencyService.GetByCode(request.CurrencyCode);

                if (currencyEntity != null) return Mapper.Map<CurrencyEntity, Currency>(currencyEntity);
            }
            catch (Exception)
            {
                return null;
            }

            return currency;
        }

        public override async Task<BaseResponse> Delete(QueryCurrencyMessage request, ServerCallContext context)
        {
            var result = new BaseResponse();

            try
            {
                await _currencyService.Delete(request.CurrencyCode);

                result.Code = ResponseCode.Success;
            }
            catch (Exception)
            {
                result.Code = ResponseCode.GeneralError;
            }

            return result;
        }
    }
}