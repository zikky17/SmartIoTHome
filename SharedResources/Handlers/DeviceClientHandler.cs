using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;
using SharedResources.Data;
using SharedResources.Factories;
using SharedResources.Models;
using System.Text;

namespace SharedResources.Handlers
{
    public class DeviceClientHandler
    {

        public SmartDeviceModel Settings { get; private set; } = new();
        private DeviceClient? _client;

        public DeviceClientHandler(string deviceId, string deviceName, string deviceType, string connectionString)
        {
            Settings!.DeviceId = deviceId;
            Settings.DeviceName = deviceName;
            Settings.DeviceType = deviceType;
            Settings.ConnectionString = connectionString;
        }

        public async Task<ResultResponse> Initialize()
        {
            var response = new ResultResponse();

            try
            {


                _client = DeviceClient.CreateFromConnectionString(Settings.ConnectionString);

                if (_client != null)
                {
                    _client.SetConnectionStatusChangesHandler(ConnectionStatusChangeHandler);

                    await Task.WhenAll(
                        _client.SetMethodDefaultHandlerAsync(DirectMethodDefaultCallback, null),
                        UpdateDeviceTwinPropertiesAsync()
                    );

                    response.Succeeded = true;
                    response.Message = "Device initialized.";
                }
                else
                {
                    response.Succeeded = false;
                    response.Message = "Device client not found.";
                }
            }
            catch (Exception ex)
            {
                response.Succeeded = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ResultResponse> DisconnectAsync(string connectionString)
        {
            var response = new ResultResponse();

            Settings.ConnectionString = connectionString;

            _client = DeviceClient.CreateFromConnectionString(Settings.ConnectionString.ToString());

            try
            {
                Settings.DeviceState = false;
                Settings.ConnectionState = false;
                UpdateDeviceTwinConnectionStateAsync(false).Wait();
                await UpdateDeviceTwinDeviceStateAsync();

                response.Succeeded = true;
                response.Message = "Device disconnected.";

            }
            catch (Exception ex)
            {
                response.Succeeded = false;
                response.Message = ex.Message;
            }

            return response;
        }



        public async Task<MethodResponse> DirectMethodDefaultCallback(MethodRequest request, object userContext)
        {
            var methodResponse = request.Name.ToLower() switch
            {
                "start" => await OnStartAsync(),
                "stop" => await OnStopAsync(),
                _ => GenerateMethodResponse("No suitable method found", 404)
            };

            return methodResponse;
        }

        public async Task<MethodResponse> OnStartAsync()
        {
            Settings.DeviceState = true;

            var result = await UpdateDeviceTwinDeviceStateAsync();
            if (result.Succeeded)
            {
                return GenerateMethodResponse("Device has successfully started.", 200);
            }
            else
                return GenerateMethodResponse($"{result.Message}", 400);
        }

        public async Task<MethodResponse> OnStopAsync()
        {
            Settings.DeviceState = false;

            var result = await UpdateDeviceTwinDeviceStateAsync();
            if (result.Succeeded)
            {
                return GenerateMethodResponse("Device has stopped.", 200);
            }
            else
                return GenerateMethodResponse($"{result.Message}", 400);
        }

        public MethodResponse GenerateMethodResponse(string message, int statusCode)
        {
            try
            {
                var json = JsonConvert.SerializeObject(new { Message = message });
                var methodResponse = new MethodResponse(Encoding.UTF8.GetBytes(json), statusCode);
                return methodResponse;
            }
            catch (Exception ex)
            {
                var json = JsonConvert.SerializeObject(new { Message = ex.Message });
                var methodResponse = new MethodResponse(Encoding.UTF8.GetBytes(json), statusCode);
                return methodResponse;
            }
        }


        public async Task<ResultResponse> UpdateDeviceTwinDeviceStateAsync()
        {
            var response = new ResultResponse();

            try
            {
                var reportedProperties = new TwinCollection
                {
                    ["deviceState"] = Settings.DeviceState
                };

                if (_client != null)
                {
                    await _client!.UpdateReportedPropertiesAsync(reportedProperties);
                    response.Succeeded = true;
                }
                else
                {
                    response.Succeeded = false;
                    response.Message = "Device client not found.";
                }
            }
            catch (Exception ex)
            {
                response.Succeeded = false;
                response.Message = ex.Message;
            }

            return response;
        }


        public async Task<ResultResponse> UpdateDeviceTwinPropertiesAsync()
        {
            var response = new ResultResponse();

            try
            {
                var reportedProperties = new TwinCollection
                {
                    ["connectionState"] = Settings.ConnectionState,
                    ["deviceName"] = Settings.DeviceName,
                    ["deviceType"] = Settings.DeviceType,
                    ["deviceState"] = Settings.DeviceState
                };

                if (_client != null)
                {
                    await _client!.UpdateReportedPropertiesAsync(reportedProperties);
                    response.Succeeded = true;
                }
                else
                {
                    response.Succeeded = false;
                    response.Message = "Device client not found.";
                }

                response.Succeeded = true;


            }
            catch (Exception ex)
            {
                response.Succeeded = false;
                response.Message = ex.Message;
            }

            return response;
        }


        public async Task<ResultResponse> UpdateDeviceTwinConnectionStateAsync(bool connectionState)
        {
            var response = new ResultResponse();

            try
            {
                var reportedProperties = new TwinCollection
                {
                    ["connectionState"] = connectionState
                };

                if (_client != null)
                {
                    await _client!.UpdateReportedPropertiesAsync(reportedProperties);
                    response.Succeeded = true;
                    response.Message = $"Device ConnectionState updated to {connectionState}";
                }
                else
                {
                    response.Succeeded = false;
                    response.Message = "Device client not found.";
                }
            }
            catch (Exception ex)
            {
                response.Succeeded = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public void ConnectionStatusChangeHandler(ConnectionStatus status, ConnectionStatusChangeReason reason)
        {
            if (status == ConnectionStatus.Disconnected || status == ConnectionStatus.Disabled)
            {
                Task.Run(() => UpdateDeviceTwinConnectionStateAsync(false));
            }
            else if (status == ConnectionStatus.Connected)
            {
                Task.Run(() => UpdateDeviceTwinConnectionStateAsync(true));
            }
        }
    }
}