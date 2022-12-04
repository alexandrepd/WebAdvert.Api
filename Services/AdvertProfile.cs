using AutoMapper;
using WebAdvert.Api.Models;

namespace WebAdvert.Api.Services
{
    public class AdvertProfile : Profile
    {
        public AdvertProfile()
        {
            CreateMap<AdvertModel, AdvertDBModel>();
        }
    }
}
