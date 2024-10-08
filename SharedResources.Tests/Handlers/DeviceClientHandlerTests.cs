using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Moq;
using SharedResources.Handlers;
using SharedResources.Models;
using System.Text;
using System.Threading.Tasks;
using Xunit;

public class DeviceClientHandlerTests
{
    private readonly Mock<DeviceClient> _mockDeviceClient;
    private readonly DeviceClientHandler _deviceClientHandler;

    public DeviceClientHandlerTests()
    {
        // Mocka DeviceClient
        _mockDeviceClient = new Mock<DeviceClient>();

        // Skapa ett DeviceClientHandler-objekt med mockade värden
        _deviceClientHandler = new DeviceClientHandler(
            "testDeviceId",
            "testDeviceName",
            "testDeviceType",
            "testConnectionString"
        );

        typeof(DeviceClientHandler)
            .GetField("_client", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(_deviceClientHandler, _mockDeviceClient.Object);
    }

    [Fact]
    public async Task Initialize_ShouldRegisterDirectMethodAndUpdateProperties()
    {
        // Arrange
        _mockDeviceClient.Setup(x => x.SetMethodDefaultHandlerAsync(It.IsAny<MethodCallback>(), null))
            .Returns(Task.CompletedTask);
        _mockDeviceClient.Setup(x => x.UpdateReportedPropertiesAsync(It.IsAny<TwinCollection>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _deviceClientHandler.Initialize();

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal("Device initialized.", result.Message);
        _mockDeviceClient.Verify(x => x.SetMethodDefaultHandlerAsync(It.IsAny<MethodCallback>(), null), Times.Once);
        _mockDeviceClient.Verify(x => x.UpdateReportedPropertiesAsync(It.IsAny<TwinCollection>()), Times.Once);
    }

    [Fact]
    public async Task DisconnectAsync_ShouldUpdateDeviceStateAndReturnSuccess()
    {
        // Arrange
        _mockDeviceClient.Setup(x => x.UpdateReportedPropertiesAsync(It.IsAny<TwinCollection>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _deviceClientHandler.DisconnectAsync("newConnectionString");

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal("Device disconnected.", result.Message);
        _mockDeviceClient.Verify(x => x.UpdateReportedPropertiesAsync(It.IsAny<TwinCollection>()), Times.Exactly(2));
    }

    [Fact]
    public async Task DirectMethodDefaultCallback_ShouldHandleStartAndStopMethods()
    {
        // Arrange
        var startRequest = new MethodRequest("start", Encoding.UTF8.GetBytes("{}"));
        var stopRequest = new MethodRequest("stop", Encoding.UTF8.GetBytes("{}"));

        _mockDeviceClient.Setup(x => x.UpdateReportedPropertiesAsync(It.IsAny<TwinCollection>()))
            .Returns(Task.CompletedTask);

        // Act
        var startResponse = await _deviceClientHandler.DirectMethodDefaultCallback(startRequest, null!);
        var stopResponse = await _deviceClientHandler.DirectMethodDefaultCallback(stopRequest, null!);

        // Assert
        Assert.Equal(200, startResponse.Status);
        Assert.Equal(200, stopResponse.Status);
    }

    [Fact]
    public async Task OnStartAsync_ShouldUpdateDeviceStateAndReturnSuccess()
    {
        // Arrange
        _mockDeviceClient.Setup(x => x.UpdateReportedPropertiesAsync(It.IsAny<TwinCollection>()))
            .Returns(Task.CompletedTask);

        // Act
        var response = await _deviceClientHandler.OnStartAsync();

        // Assert
        var responseBody = Encoding.UTF8.GetString(response.Result);
        Assert.Equal(200, response.Status);
        Assert.Equal("Device has successfully started.", responseBody);
        _mockDeviceClient.Verify(x => x.UpdateReportedPropertiesAsync(It.IsAny<TwinCollection>()), Times.Once);
    }

    [Fact]
    public async Task OnStopAsync_ShouldUpdateDeviceStateAndReturnSuccess()
    {
        // Arrange
        _mockDeviceClient.Setup(x => x.UpdateReportedPropertiesAsync(It.IsAny<TwinCollection>()))
            .Returns(Task.CompletedTask);

        // Act
        var response = await _deviceClientHandler.OnStopAsync();

        // Assert
        var responseBody = Encoding.UTF8.GetString(response.Result);
        Assert.Equal(200, response.Status);
        Assert.Equal("Device has stopped.", responseBody);
        _mockDeviceClient.Verify(x => x.UpdateReportedPropertiesAsync(It.IsAny<TwinCollection>()), Times.Once);
    }

    [Fact]
    public async Task UpdateDeviceTwinPropertiesAsync_ShouldUpdateReportedProperties()
    {
        // Arrange
        _mockDeviceClient.Setup(x => x.UpdateReportedPropertiesAsync(It.IsAny<TwinCollection>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _deviceClientHandler.UpdateDeviceTwinPropertiesAsync();

        // Assert
        Assert.True(result.Succeeded);
        _mockDeviceClient.Verify(x => x.UpdateReportedPropertiesAsync(It.IsAny<TwinCollection>()), Times.Once);
    }

    [Fact]
    public async Task UpdateDeviceTwinConnectionStateAsync_ShouldUpdateConnectionState()
    {
        // Arrange
        _mockDeviceClient.Setup(x => x.UpdateReportedPropertiesAsync(It.IsAny<TwinCollection>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _deviceClientHandler.UpdateDeviceTwinConnectionStateAsync(true);

        // Assert
        Assert.True(result.Succeeded);
        _mockDeviceClient.Verify(x => x.UpdateReportedPropertiesAsync(It.IsAny<TwinCollection>()), Times.Once);
    }
}
