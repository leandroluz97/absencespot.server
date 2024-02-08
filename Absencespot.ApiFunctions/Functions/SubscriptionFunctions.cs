using System;
using System.IO;
using System.Linq;
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

            //var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
            //if (string.IsNullOrWhiteSpace(query["customerId"]))
            //{
            //    throw new ArgumentNullException("customerId");
            //}
            //if (string.IsNullOrWhiteSpace(query["priceId"]))
            //{
            //    throw new ArgumentNullException("priceId");
            //}

            var result = await _subscriptionService.GetAllAsync(companyId);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(result, _objectSerializer)
              .ConfigureAwait(false);
            return response;
        }

        [Function(nameof(CreateCustomer))]
        public async Task<HttpResponseData> CreateCustomer([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "companies/{companyId}/customer")]
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

        [Function(nameof(AttachCustomerPaymentMethodAsync))]
        public async Task<HttpResponseData> AttachCustomerPaymentMethodAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "companies/{companyId}/payment-method")]
        HttpRequestData req, Guid companyId)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var paymentMethodBody = JsonSerializer.Deserialize<Dtos.AttachPaymentMethod>(req.Body, _jsonSerializerOptions);
            await _subscriptionService.AttachCustomerPaymentMethodAsync(companyId, paymentMethodBody.PaymentMethodId);

            var response = req.CreateResponse(HttpStatusCode.Created);
            return response;
        }

        [Function(nameof(CreateSetupIntent))]
        public async Task<HttpResponseData> CreateSetupIntent([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "companies/{companyId}/subscriptions/setup-intent")]
        HttpRequestData req, Guid companyId)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var result = await _subscriptionService.CreateSetupIntentAsync(companyId);

            var response = req.CreateResponse(HttpStatusCode.Created);
            await response.WriteAsJsonAsync(result, _objectSerializer, statusCode: HttpStatusCode.Created)
                .ConfigureAwait(false);
            return response;
        }

        [Function(nameof(UpdateSubscription))]
        public async Task<HttpResponseData> UpdateSubscription([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "companies/{companyId}/subscriptions/{subscriptionId}")]
        HttpRequestData req, Guid companyId)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var subscriptionBody = JsonSerializer.Deserialize<Dtos.UpdateSubscription>(req.Body, _jsonSerializerOptions);
            await _subscriptionService.UpdateAsync(companyId, subscriptionBody);

            var response = req.CreateResponse(HttpStatusCode.OK);
            return response;
        }


        [Function(nameof(UpgradeSubscription))]
        public async Task<HttpResponseData> UpgradeSubscription([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "companies/{companyId}/subscriptions/{subscriptionId}/upgrade")]
        HttpRequestData req, Guid companyId)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var subscriptionBody = JsonSerializer.Deserialize<Dtos.UpgradeSubscription>(req.Body, _jsonSerializerOptions);
            var result = await _subscriptionService.UpgradeAsync(companyId, subscriptionBody);

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


        [Function(nameof(GetPaymentMethod))]
        public async Task<HttpResponseData> GetPaymentMethod([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "companies/{companyId}/payment-method")]
        HttpRequestData req, Guid companyId)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var result = await _subscriptionService.GetPaymentMethodAsync(companyId);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(result, _objectSerializer)
              .ConfigureAwait(false);
            return response;
        }


        [Function(nameof(GetPrices))]
        public async Task<HttpResponseData> GetPrices([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "companies/{companyId}/subscriptions/prices")]
        HttpRequestData req, Guid companyId)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var result = await _subscriptionService.GetPricesAsync();

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(result, _objectSerializer)
                .ConfigureAwait(false);
            return response;
        }

        [Function(nameof(GetPaymentHistory))]
        public async Task<HttpResponseData> GetPaymentHistory([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "companies/{companyId}/payment-history")]
        HttpRequestData req, Guid companyId)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var result = await _subscriptionService.GetInvoicesAsync(companyId);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(result, _objectSerializer)
                .ConfigureAwait(false);
            return response;
        }


        [Function(nameof(Events))]
        public async Task<HttpResponseData> Events([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "events")]
        HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var json = await new StreamReader(req.Body)
                .ReadToEndAsync();

            const string endpointSecret = "whsec_2a5eab0370d685c4a9ebf9d24d08dd74bcf513304c38740ac33e5df93edaa08d";

            var stripeEvent = Stripe.EventUtility.ParseEvent(json);
            if (!req.Headers.TryGetValues("Stripe-Signature", out var values))
            {
                throw new ArgumentException("Stripe-Signature");
            }
            string signatureHeader = values.FirstOrDefault();

            stripeEvent = Stripe.EventUtility.ConstructEvent(json, signatureHeader, endpointSecret);

            await _subscriptionService.Events(stripeEvent);

            var response = req.CreateResponse(HttpStatusCode.Created);
            return response;
        }
    }
}
