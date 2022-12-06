using WebAdvert.Api.Models;

namespace WebAdvert.Api.Services
{
    public interface IAdvertStorageService
    {
        Task<string> Add(AdvertModel model);
        Task<bool> Confirm(ConfirmAdvertModel model);

        Task<bool> CheckAdvertTableAsync();
    }
}
