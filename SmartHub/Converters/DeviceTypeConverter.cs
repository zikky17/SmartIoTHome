using System.Diagnostics;
using System.Globalization;

namespace SmartHub.Converters
{
    public class DeviceTypeConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string deviceType)
            {
                return deviceType.ToLower() switch
                {
                    "light" => "fa-regular fa-lightbulb",
                    "fan" => "fa-regular fa-fan",
                    "temp" => "fa-sharp fa-light fa-temperature-high",
                    _ => "fa-regular fa-microchip"
                };
            }
            return "fa-regular fa-microchip";
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null!;
            }
        }
    }
}
