using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Absencespot.Business.Abstractions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Absencespot.ApiFunctions.Functions
{

    public class SubscriptionFunctions : BaseFunction
    {
        private readonly ILogger _logger;
        private readonly ISubscriptionService _subscriptionService;

        public SubscriptionFunctions(ILogger<SettingsFunctions> logger, ISubscriptionService subscriptionService) : base(logger)
        {
            _logger = logger;
            _subscriptionService = subscriptionService;
        }


        [Function(nameof(GetAllSubscriptions))]
        public async Task<HttpResponseData> GetAllSubscriptions([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "companies/{companyId}/subscriptions")]
        HttpRequestData req, Guid companyId)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
            if (string.IsNullOrWhiteSpace(query["customerId"]))
            {
                throw new ArgumentNullException("customerId");
            }
            if (string.IsNullOrWhiteSpace(query["priceId"]))
            {
                throw new ArgumentNullException("priceId");
            }

            var result = await _subscriptionService.GetAllAsync(companyId, query["customerId"], query["priceId"]);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(result, _objectSerializer)
              .ConfigureAwait(false);
            return response;
        }

        [Function(nameof(CreateCustomer))]
        public async Task<HttpResponseData> CreateCustomer([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "companies/{companyId}/subscriptions/customer")]
        HttpRequestData req, Guid companyId)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var customerBody = JsonSerializer.Deserialize<Dtos.Customer>(req.Body, _jsonSerializerOptions);
            var result = await _subscriptionService.CreateCustomerAsync(companyId, customerBody);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(result, _objectSerializer)
                          .ConfigureAwait(false);
            return response;
        }

        //[Function(nameof(CreatePaymentIntent))]
        //public async Task<HttpResponseData> CreatePaymentIntent([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "companies/{companyId}/subscriptions/payment-intents")]
        //HttpRequestData req, Guid companyId)
        //{
        //    _logger.LogInformation("C# HTTP trigger function processed a request.");

        //    var subscriptionBody = JsonSerializer.Deserialize<Dtos.CreatePaymentIntent>(req.Body, _jsonSerializerOptions);
        //    var result = await _subscriptionService.CreatePaymentIntentAsync(companyId, subscriptionBody);

        //    var response = req.CreateResponse(HttpStatusCode.OK);
        //    await response.WriteAsJsonAsync(result, _objectSerializer)
        //                  .ConfigureAwait(false);
        //    return response;
        //}

        [Function(nameof(CreateSubscription))]
        public async Task<HttpResponseData> CreateSubscription([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "companies/{companyId}/subscriptions")]
        HttpRequestData req, Guid companyId)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var subscriptionBody = JsonSerializer.Deserialize<Dtos.CreateSubscription>(req.Body, _jsonSerializerOptions);
            var result = await _subscriptionService.CreateAsync(companyId, subscriptionBody);

            var response = req.CreateResponse(HttpStatusCode.Created);
            await response.WriteAsJsonAsync(result, _objectSerializer)
                .ConfigureAwait(false);
            return response;
        }


        [Function(nameof(CancelSubscription))]
        public async Task<HttpResponseData> CancelSubscription([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "companies/{companyId}/subscriptions/{subscriptionId}/cancel")]
        HttpRequestData req, Guid companyId, string subscriptionId) 
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            await _subscriptionService.CancelAsync(companyId, subscriptionId);

            var response = req.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        [Function(nameof(GetPublishableKey))]
        public async Task<HttpResponseData> GetPublishableKey([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "companies/{companyId}/subscriptions/configuration")]
        HttpRequestData req, Guid companyId)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var result = await _subscriptionService.GetPublishableKeyAsync();

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(result, _objectSerializer)
              .ConfigureAwait(false);
            return response;
        }


        [Function(nameof(GetProducts))]
        public async Task<HttpResponseData> GetProducts([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "companies/{companyId}/subscriptions/products")]
        HttpRequestData req, Guid companyId)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var result = await _subscriptionService.GetPricesAsync();

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(result, _objectSerializer)
                .ConfigureAwait(false);
            return response;
        }

    }
}
