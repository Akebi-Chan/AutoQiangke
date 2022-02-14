using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace MaterialDesignThemes.Wpf.Converters
{
    public class BrushRoundConverter : IValueConverter
    {
        public Brush? HighValue { get; set; } = Brushes.White;

        public Brush? LowValue { get; set; } = Brushes.Black;

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var solidColorBrush = value as SolidColorBrush;
            if (solidColorBrush is null) return null;

            var color = solidColorBrush.Color;

            var brightness = 0.3 * color.R + 0.59 * color.G + 0.11 * color.B;

            return brightness < 123 ? LowValue : HighValue;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => Binding.DoNothing;
    }

    public sealed class MathMultipleConverter : IMultiValueConverter
    {
        public MathOperation Operation { get; set; }

        public object? Convert(object?[]? value, Type? targetType, object? parameter, CultureInfo? culture)
        {
            if (value is null || value.Length < 2 || value[0] is null || value[1] is null) return Binding.DoNothing;

            if (!double.TryParse(value[0]!.ToString(), out double value1) || !double.TryParse(value[1]!.ToString(), out double value2))
                return 0;

            switch (Operation)
            {
                default:
                    // (case MathOperation.Add:)
                    return value1 + value2;
                case MathOperation.Divide:
                    return value1 / value2;
                case MathOperation.Multiply:
                    return value1 * value2;
                case MathOperation.Subtract:
                    return value1 - value2;
                case MathOperation.Pow:
                    return Math.Pow(value1, value2);
            }

        }

        public object?[]? ConvertBack(object? value, Type[]? targetTypes, object? parameter, CultureInfo? culture)
            => throw new NotImplementedException();
    }

    public class NullableToVisibilityConverter : IValueConverter
    {
        public Visibility NullValue { get; set; } = Visibility.Collapsed;
        public Visibility NotNullValue { get; set; } = Visibility.Visible;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? NullValue : NotNullValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
