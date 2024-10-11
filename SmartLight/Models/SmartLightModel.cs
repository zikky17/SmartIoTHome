using CommunityToolkit.Mvvm.ComponentModel;

namespace SmartLight.Models
{
    public partial class SmartLightModel : ObservableObject
    {
        [ObservableProperty]
        public bool hasSettings;

        [ObservableProperty]
        public string? id;

        [ObservableProperty]
        public string? location;

        [ObservableProperty]
        public string? type;

        [ObservableProperty]
        public string? connectionString;

        [ObservableProperty]
        public string? deviceState;
    }
}
