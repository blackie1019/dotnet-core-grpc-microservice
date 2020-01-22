using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Grpc.Core;
using MockSite.Core.Entities;
using MockSite.Core.Interfaces;
using MockSite.Message;

namespace MockSite.DomainService
{
    public class LocalizationServiceImpl : LocalizationService.LocalizationServiceBase
    {
        private readonly ILocalizationService _localizationService;

        public LocalizationServiceImpl(ILocalizationService localizationService)
        {
            _localizationService = localizationService;
        }

        public override async Task<BaseResponse> Modify(Language request, ServerCallContext context)
        {
            var result = new BaseResponse();
            var languages = new List<LanguageEntity>();

            try
            {
                languages.Add(new LanguageEntity
                {
                    DisplayValue = request.DisplayValue,
                    LangCode = request.LangCode
                });

                await _localizationService.Modify(new LocalizationEntity
                {
                    DisplayKey = request.DisplayKey,
                    LanguageSets = languages
                });

                result.Code = ResponseCode.Success;
            }
            catch (Exception)
            {
                result.Code = ResponseCode.GeneralError;
            }

            return result;
        }

        public override async Task<Languages> GetAll(Empty request, ServerCallContext context)
        {
            try
            {
                var localizationEntities = await _localizationService.GetAll();

                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<IEnumerable<LocalizationEntity>, Languages>()
                        .ForMember(d => d.Value, opt => opt.MapFrom(src => src));
                    cfg.CreateMap<LocalizationEntity, LanguageSet>()
                        .ForMember(d => d.LanguageSets, opt => opt.MapFrom(src => src.LanguageSets));
                });

                var mapper = config.CreateMapper();

                mapper.ConfigurationProvider.AssertConfigurationIsValid();

                return mapper.Map<IEnumerable<LocalizationEntity>, Languages>(localizationEntities);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public override async Task<LanguageSet> Get(QueryLanguageMessage request, ServerCallContext context)
        {
            try
            {
                var localizationEntity = await _localizationService.GetByCode(request.DisplayKey);

                var config = new MapperConfiguration(cfg => cfg.CreateMap<LocalizationEntity, LanguageSet>()
                    .ForMember(d => d.LanguageSets, opt => opt.MapFrom(src => src.LanguageSets))
                );

                var mapper = config.CreateMapper();

                return mapper.Map<LocalizationEntity, LanguageSet>(localizationEntity);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public override async Task<BaseResponse> Delete(QueryLanguageMessage request, ServerCallContext context)
        {
            var result = new BaseResponse();

            try
            {
                await _localizationService.Delete(request.DisplayKey);

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