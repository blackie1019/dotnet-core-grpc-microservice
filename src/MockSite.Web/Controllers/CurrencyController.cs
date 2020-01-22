#region

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MockSite.Common.Core.Models;
using MockSite.Message;
using MockSite.Web.Constants;
using ResponseCode = MockSite.Common.Core.Enums.ResponseCode;

#endregion

namespace MockSite.Web.Controllers
{
    [Route("api/[Controller]")]
    public class CurrencyController : ControllerBase
    {
        private readonly CurrencyService.CurrencyServiceClient _serviceClient;

        public CurrencyController(CurrencyService.CurrencyServiceClient serviceClient)
        {
            _serviceClient = serviceClient;
        }

        [Authorize(Roles = Policy.CommonReadonly)]
        [HttpGet("GetCurrencyAll")]
        public async Task<ResponseBaseModel<IEnumerable<Currency>>> GetCurrencyAll()
        {
            var result = await _serviceClient.GetCurrencyAllAsync(new Empty());

            return new ResponseBaseModel<IEnumerable<Currency>>(ResponseCode.Success, result.Value);
        }

        [Authorize(Roles = Policy.CommonReadonly)]
        [HttpGet("GetCurrency/{currencyCode}")]
        public async Task<ResponseBaseModel<Currency>> GetCurrency(string currencyCode)
        {
            var result = await _serviceClient.GetAsync(new QueryCurrencyMessage {CurrencyCode = currencyCode});

            return new ResponseBaseModel<Currency>(ResponseCode.Success, result);
        }

        [Authorize(Roles = Policy.CommonModify)]
        [HttpPost("CreateCurrency")]
        public async Task<ResponseBaseModel<string>> CreateCurrency([FromBody] CreateCurrency request)
        {
            var result = new BaseResponse();

            var currency = await _serviceClient.GetAsync(new QueryCurrencyMessage
                {CurrencyCode = request.CurrencyCode});

            if (request.IfExistUpdateRate || currency == null)
            {
                result = await _serviceClient.ModifyAsync(new Currency
                {
                    CurrencyCode = request.CurrencyCode,
                    CurrencyRate = request.CurrencyRate
                });
            }

            return new ResponseBaseModel<string>((ResponseCode) result.Code, null);
        }

        [Authorize(Roles = Policy.CommonModify)]
        [HttpPost("UpdateCurrency")]
        public async Task<ResponseBaseModel<string>> UpdateCurrency([FromBody] UpdateCurrency request)
        {
            var result = await _serviceClient.ModifyAsync(new Currency
            {
                CurrencyCode = request.CurrencyCode,
                CurrencyRate = request.CurrencyRate
            });

            return new ResponseBaseModel<string>((ResponseCode) result.Code, null);
        }

        [Authorize(Roles = Policy.CommonDelete)]
        [HttpPost("DeleteCurrency/{currencyCode}")]
        public async Task<ResponseBaseModel<string>> DeleteCurrency(string currencyCode)
        {
            var result = await _serviceClient.DeleteAsync(new QueryCurrencyMessage {CurrencyCode = currencyCode});

            return new ResponseBaseModel<string>((ResponseCode) result.Code, null);
        }

        [Authorize(Roles = Policy.CommonReadonly)]
        [HttpGet("GetTtlCurrency")]
        public async Task<ResponseBaseModel<IEnumerable<Currency>>> GetTtlCurrency()
        {
            var result = await _serviceClient.GetTtlCurrenciesAsync(new Empty());

            return new ResponseBaseModel<IEnumerable<Currency>>(ResponseCode.Success, result.Value);
        }
    }
}