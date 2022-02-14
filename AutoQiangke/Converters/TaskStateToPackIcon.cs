using AutoQiangke.Models;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace AutoQiangke.Converters
{
    public class TaskStateToPackIcon : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TaskState state)
            {
                switch (state)
                {
                    case TaskState.None:
                        return PackIconKind.Play;
                    case TaskState.Wait:
                        return PackIconKind.Alarm;
                    case TaskState.Started:
                        return PackIconKind.Stop;
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
