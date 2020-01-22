using System.Collections.Generic;
using System.Threading.Tasks;
using MockSite.Core.Entities;

namespace MockSite.Core.Interfaces
{
    public interface ILocalizationRepository
    {
        Task Insert(LocalizationEntity localizationEntity);

        Task Update(LocalizationEntity localizationEntity);

        Task Delete(string code);

        Task<LocalizationEntity> GetByCode(string code);

        Task<IEnumerable<LocalizationEntity>> GetAll();
    }
}