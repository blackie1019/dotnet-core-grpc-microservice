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
    public class LocalizationController : ControllerBase
    {
        private readonly LocalizationService.LocalizationServiceClient _serviceClient;

        public LocalizationController(LocalizationService.LocalizationServiceClient serviceClient)
        {
            _serviceClient = serviceClient;
        }

        [Authorize(Roles = Policy.CommonReadonly)]
        [HttpGet("GetLanguages")]
        public async Task<ResponseBaseModel<IEnumerable<LanguageSet>>> GetLanguages()
        {
            var result = await _serviceClient.GetAllAsync(new Empty());

            return new ResponseBaseModel<IEnumerable<LanguageSet>>(ResponseCode.Success, result.Value);
        }

        [Authorize(Roles = Policy.CommonReadonly)]
        [HttpGet("GetLanguage/{displayKey}")]
        public async Task<ResponseBaseModel<LanguageSet>> GetLanguage(string displayKey)
        {
            var result = await _serviceClient.GetAsync(new QueryLanguageMessage {DisplayKey = displayKey});

            return new ResponseBaseModel<LanguageSet>(ResponseCode.Success, result);
        }

        [Authorize(Roles = Policy.CommonModify)]
        [HttpPost("ModifyLanguage")]
        public async Task<ResponseBaseModel<string>> ModifyLanguage([FromBody] Language request)
        {
            var result = await _serviceClient.ModifyAsync(request);

            return new ResponseBaseModel<string>((ResponseCode) result.Code, null);
        }

        [Authorize(Roles = Policy.CommonDelete)]
        [HttpPost("DeleteLanguage/{displayKey}")]
        public async Task<ResponseBaseModel<string>> DeleteLanguage(string displayKey)
        {
            var result = await _serviceClient.DeleteAsync(new QueryLanguageMessage {DisplayKey = displayKey});

            return new ResponseBaseModel<string>((ResponseCode) result.Code, null);
        }
    }
}