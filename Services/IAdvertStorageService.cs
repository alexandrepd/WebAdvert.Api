using WebAdvert.Api.Models;

namespace WebAdvert.Api.Services
{
    public interface IAdvertStorageService
    {
        Task<string> AddAsync(AdvertModel model);
        Task ConfirmAsync(ConfirmAdvertModel model);
        Task<AdvertModel> GetByIdAsync(string id);
        Task<bool> CheckHealthAsync();
        Task<List<AdvertModel>> GetAllAsync();
        Task DeleteAsync(string id);
        Task<AdvertModel> UpdateAsync(AdvertModel model);
    }
}
