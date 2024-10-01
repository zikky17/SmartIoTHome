using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SharedResources.Handlers;
using SharedResources.Managers;
using SharedResources.Models;

namespace AzureFunctions.Functions
{
    public class RegisterDevice
    {
        private readonly ILogger<RegisterDevice> _logger;
        private readonly IoTHubManager _hub;

        public RegisterDevice(ILogger<RegisterDevice> logger, IoTHubManager hub)
        {
            _logger = logger;
            _hub = hub;
        }

        [Function("DeviceRegistration")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            var body = await new StreamReader(req.Body).ReadToEndAsync();
            var drr = JsonConvert.DeserializeObject<DeviceRegistrationRequest>(body);

            if (drr == null || string.IsNullOrEmpty(drr.DeviceId) || string.IsNullOrEmpty(drr.DeviceName))
                return new BadRequestObjectResult("Invalid request. 'deviceId' or 'deviceName' is missing.");

            var result = await _hub.RegisterDeviceAsync(drr.DeviceId, drr.DeviceName);

            var response = new DeviceRegistrationResponse
            {
                DeviceId = result.Device?.Id,
                ConnectionString = result.ConnectionString,
                DeviceName = result.Twin?.Properties.Desired["deviceName"].ToString(),
                DeviceState = result.Twin?.Properties.Desired["deviceState"]
                
            };

            return new OkObjectResult(response);
        }
    }
}
