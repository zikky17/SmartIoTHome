using CommunityToolkit.Mvvm.ComponentModel;

namespace SmartTemperature.Models
{
    public partial class SmartTempModel : ObservableObject
    {
        [ObservableProperty]
        public bool hasSettings;

        [ObservableProperty]
        public string id;

        [ObservableProperty]
        public string location;

        [ObservableProperty]
        public string type;

        [ObservableProperty]
        public string connectionString;
    }
}
