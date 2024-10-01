using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SharedResources.Managers;

namespace AzureFunctions.Functions
{
    public class DeleteDevice
    {
        private readonly ILogger<DeleteDevice> _logger;
        private readonly IoTHubManager _hub;

        public DeleteDevice(ILogger<DeleteDevice> logger, IoTHubManager hub)
        {
            _logger = logger;
            _hub = hub;
        }

        [Function("DeleteDevice")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "delete", Route = "DeleteDevice")] HttpRequest req, ILogger log)
        {
            string deviceId = req.Query["deviceId"];

            if (string.IsNullOrEmpty(deviceId))
            {
                return new BadRequestObjectResult("Device ID is required.");
            }

            try
            {
                var result = await _hub.RemoveDeviceAsync(deviceId);
                return new OkObjectResult($"Device {deviceId} successfully removed.");
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult($"Error removing device: {ex.Message}");
            }
        }
    }
}
