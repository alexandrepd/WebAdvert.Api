using Amazon.DynamoDBv2.DataModel;
using WebAdvert.Api.Models;

namespace WebAdvert.Api.Services
{
    [DynamoDBTable("Adverts")]
    public class AdvertDBModel
    {
        [DynamoDBHashKey]
        public string? Id { get; set; }
        [DynamoDBProperty]
        public string? Title { get; set; }
        [DynamoDBProperty]
        public string? Description { get; set; }
        [DynamoDBProperty]
        public double Price { get; set; }
        [DynamoDBProperty]
        public DateTime CreationDateTime { get; set; }
        [DynamoDBProperty]
        public AdvertStatus Status { get; set; }


    }
}
