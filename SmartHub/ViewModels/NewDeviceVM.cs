﻿using SharedResources.Models;

namespace SmartHub.ViewModels;

public class NewDeviceVM
{
    public DeviceRegistrationRequest? RegistrationRequest { get; set; } = new DeviceRegistrationRequest();
    public DeviceRegistrationResponse? RegistrationResponse { get; set; }
}
