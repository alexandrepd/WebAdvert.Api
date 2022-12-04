using AutoMapper;
using WebAdvert.Api.Models;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;

namespace WebAdvert.Api.Services
{
    public class DynamoDBAdvertStorage : IAdvertStorageService
    {

        private readonly IMapper _mapper;

        public DynamoDBAdvertStorage(IMapper mapper)
        {
            _mapper = mapper;
        }
        public async Task<string> Add(AdvertModel model)
        {
            AdvertDBModel _dbModel = _mapper.Map<AdvertDBModel>(model);

            _dbModel.Id = new Guid().ToString();
            _dbModel.CreationDateTime = DateTime.UtcNow;
            _dbModel.Status = AdvertStatus.Pending;


            using (var client = new AmazonDynamoDBClient())
            {
                using (var context = new DynamoDBContext(client))
                {
                    await context.SaveAsync<AdvertDBModel>(_dbModel);
                }
            }
            return _dbModel.Id;
        }

        public async Task<bool> Confirm(ConfirmAdvertModel model)
        {
            using (var client = new AmazonDynamoDBClient())
            {
                using (var context = new DynamoDBContext(client))
                {
                    var _record = await context.LoadAsync<AdvertDBModel>(model.Id);
                    if (_record == null)
                    {
                        throw new KeyNotFoundException($"A record with ID = {model.Id} was not found.");
                    }
                    if (model.Status == AdvertStatus.Active)
                    {
                        _record.Status = AdvertStatus.Active;
                        await context.SaveAsync<AdvertDBModel>(_record);
                    }
                    else
                    {
                        await context.DeleteAsync<AdvertDBModel>(_record);
                    }
                }
            }

            return true;
        }
    }
}
