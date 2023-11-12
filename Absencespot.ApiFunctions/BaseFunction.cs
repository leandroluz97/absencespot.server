using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using Azure.Core.Serialization;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Absencespot.ApiFunctions
{
    public  class BaseFunction
    {
        protected readonly ILogger _logger;
        protected readonly JsonSerializerOptions _jsonSerializerOptions;
        protected readonly ObjectSerializer _objectSerializer;

        public BaseFunction(ILogger<BaseFunction> logger)
        {
            _logger = logger;
            _jsonSerializerOptions = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
                AllowTrailingCommas = false,
            };
            _objectSerializer = new JsonObjectSerializer(_jsonSerializerOptions);
        }

    }
}
