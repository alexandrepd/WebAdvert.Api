using Microsoft.AspNetCore.Mvc;
using WebAdvert.Api.Services;
using WebAdvert.Api.Models;
using WebAdvert.Api.Models.Messages;
using Amazon.SimpleNotificationService;
using Newtonsoft.Json;
using Amazon.Runtime;
using Amazon;
using Microsoft.AspNetCore.Cors;
using Amazon.SimpleNotificationService.Model;

namespace WebAdvert.Api.Controllers
{
    [ApiController]
    [Route("adverts/v1")]
    public class Advert : ControllerBase
    {
        private readonly IAdvertStorageService _advertStorageService;
        private readonly IConfiguration _configuration;
        private readonly RegionEndpoint _region = Amazon.RegionEndpoint.USEast1;
        private readonly BasicAWSCredentials credentials;

        public Advert(IAdvertStorageService advertStorageService, IConfiguration configuration)
        {
            _advertStorageService = advertStorageService;
            _configuration = configuration;

            credentials = new BasicAWSCredentials(_configuration["AWS:AwsAccessKeyId"], _configuration["AWS:AwsSecretAccessKey"]);
        }


        [HttpPost]
        [Route("Create")]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(CreateAdvertResponse))]
        public async Task<IActionResult> Create(AdvertModel model)
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            string _recordId;
            try
            {
                _recordId = await _advertStorageService.AddAsync(model);
            }
            catch (KeyNotFoundException)
            {
                return new NotFoundResult();
            }
            catch (Exception exception)
            {
                return StatusCode(500, exception.Message);
            }

            return StatusCode(201, new CreateAdvertResponse() { Id = _recordId });
        }

        [HttpPut]
        [Route("Confirm")]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        public async Task<IActionResult> ConfirmAsync(ConfirmAdvertModel model)
        {

            try
            {
                await _advertStorageService.ConfirmAsync(model);
                await RaiseAdvertConfirmedMessage(model);
            }
            catch (KeyNotFoundException)
            {
                return new NotFoundResult();
            }
            catch (Exception exception)
            {
                return StatusCode(500, exception.Message);
            }

            return new OkResult();
        }

        [HttpPost]
        [Route("healthCheck")]
        [ProducesResponseType(400)]
        [ProducesResponseType(200)]
        public async Task<IActionResult> healthCheck()
        {

            bool isAlive;
            try
            {
                isAlive = await _advertStorageService.CheckHealthAsync();
            }
            catch (KeyNotFoundException)
            {
                return new NotFoundResult();
            }
            catch (Exception exception)
            {
                return StatusCode(500, exception.Message);
            }

            return StatusCode(201);
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Get(string id)
        {
            try
            {
                var advert = await _advertStorageService.GetByIdAsync(id);
                return new JsonResult(advert);
            }
            catch (KeyNotFoundException)
            {
                return new NotFoundResult();
            }
            catch (Exception)
            {
                return new StatusCodeResult(500);
            }
        }

        [HttpGet]
        [Route("all")]
        [ProducesResponseType(200)]
        //[EnableCors("AllOrigin")]
        public async Task<IActionResult> All()
        {
            return new JsonResult(await _advertStorageService.GetAllAsync());
        }

        private async Task RaiseAdvertConfirmedMessage(ConfirmAdvertModel model)
        {
            string TopicArn = _configuration.GetValue<string>("TopicArn");
            AdvertModel dbModel = await _advertStorageService.GetByIdAsync(model.Id);

            using (var client = new AmazonSimpleNotificationServiceClient(credentials: credentials, region: _region))
            {
                var message = new AdvertConfirmedMessage
                {
                    Id = model.Id,
                    Title = dbModel.Title
                };

                string messageJson = JsonConvert.SerializeObject(message);
                await client.PublishAsync(TopicArn, messageJson);
            }
        }
    }
}
