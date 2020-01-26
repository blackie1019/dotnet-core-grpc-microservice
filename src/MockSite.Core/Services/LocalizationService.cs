using System.Collections.Generic;
using System.Threading.Tasks;
using MockSite.Core.Entities;
using MockSite.Core.Interfaces;

namespace MockSite.Core.Services
{
    public class LocalizationService : ILocalizationService
    {
        private readonly ILocalizationRepository _localizationRepository;

        public LocalizationService(ILocalizationRepository localizationRepository)
        {
            _localizationRepository = localizationRepository;
        }

        public async Task Modify(LocalizationEntity localEntity)
        {
            var document = await _localizationRepository.GetByCode(localEntity.DisplayKey);

            if (document == null)
                await _localizationRepository.Insert(localEntity);
            else
                await _localizationRepository.Update(localEntity);
        }

        public async Task Delete(string code)
        {
            await _localizationRepository.Delete(code);
        }

        public async Task<LocalizationEntity> GetByCode(string code)
        {
            return await _localizationRepository.GetByCode(code);
        }

        public async Task<IEnumerable<LocalizationEntity>> GetAll()
        {
            return await _localizationRepository.GetAll();
        }
    }
}