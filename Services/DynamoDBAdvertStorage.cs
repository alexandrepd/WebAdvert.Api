using AutoMapper;
using WebAdvert.Api.Models;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon;
using Amazon.Runtime;
using System.Reflection;

namespace WebAdvert.Api.Services
{
    public class DynamoDBAdvertStorage : IAdvertStorageService
    {

        private readonly IMapper _mapper;
        private readonly RegionEndpoint _region = Amazon.RegionEndpoint.USEast1;
        private readonly IConfiguration Configuration;
        private readonly BasicAWSCredentials credentials;

        public DynamoDBAdvertStorage(IMapper mapper, IConfiguration configuration)
        {
            _mapper = mapper;
            Configuration = configuration;
            credentials = new BasicAWSCredentials(Configuration["AWS:AwsAccessKeyId"], Configuration["AWS:AwsSecretAccessKey"]);
        }
        public async Task<string> Add(AdvertModel model)
        {
            AdvertDBModel _dbModel = _mapper.Map<AdvertDBModel>(model);

            _dbModel.Id = Guid.NewGuid().ToString();
            _dbModel.CreationDateTime = DateTime.UtcNow;
            _dbModel.Status = AdvertStatus.Pending;


            using (var client = new AmazonDynamoDBClient(credentials, region: _region))
            {
                using (var context = new DynamoDBContext(client))
                {
                    await context.SaveAsync<AdvertDBModel>(_dbModel);
                }
            }
            return _dbModel.Id;
        }

        public async Task<bool> CheckAdvertTableAsync()
        {
            using (var client = new AmazonDynamoDBClient(credentials, region: _region))
            {
                var tableData = await client.DescribeTableAsync("Adverts");
                return string.Compare(tableData.Table.TableStatus, "active", true) == 0;
            }
        }

        public async Task<bool> Confirm(ConfirmAdvertModel model)
        {
            using (var client = new AmazonDynamoDBClient(credentials, region: _region))
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

        public async Task<AdvertDBModel> GetById(string id)
        {
            using (var client = new AmazonDynamoDBClient(credentials, region: _region))
            {
                using (var context = new DynamoDBContext(client))
                {
                    return await context.LoadAsync<AdvertDBModel>(id);
                }
            }

        }
    }
}
