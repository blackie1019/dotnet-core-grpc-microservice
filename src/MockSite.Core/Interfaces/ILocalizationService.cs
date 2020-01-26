using System.Collections.Generic;
using System.Threading.Tasks;
using MockSite.Core.Entities;

namespace MockSite.Core.Interfaces
{
    public interface ILocalizationService
    {
        Task Modify(LocalizationEntity localizationEntity);

        Task Delete(string code);

        Task<LocalizationEntity> GetByCode(string code);

        Task<IEnumerable<LocalizationEntity>> GetAll();
    }
}