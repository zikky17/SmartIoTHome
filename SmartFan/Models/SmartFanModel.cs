using CommunityToolkit.Mvvm.ComponentModel;

namespace SmartFan.Models
{
    public partial class SmartFanModel : ObservableObject
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
