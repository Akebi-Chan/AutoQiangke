using AutoQiangke.Helpers;
using AutoQiangke.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace AutoQiangke.Converters
{
    class JxbStateToColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is JxbStateEnum state)
            {
                switch (state)
                {
                    case JxbStateEnum.Chosen:
                        return new SolidColorBrush(ThemeHelper.StringToColor("#4CAF50"));//绿色
                    case JxbStateEnum.Error:
                        return new SolidColorBrush(ThemeHelper.StringToColor("#F44336"));//红色
                    case JxbStateEnum.NotFound:
                        return new SolidColorBrush(ThemeHelper.StringToColor("#FF5722"));//橙色
                    case JxbStateEnum.Ready:
                        return new SolidColorBrush(ThemeHelper.StringToColor("#2196F3"));//蓝色
                    case JxbStateEnum.Unknow:
                        return new SolidColorBrush(ThemeHelper.StringToColor("#607D8B"));//灰色
                    case JxbStateEnum.Known:
                        return new SolidColorBrush(ThemeHelper.StringToColor("#673AB7"));//紫色
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public static class JxbStateToBrush
    {
        public static SolidColorBrush Convert(JxbStateEnum state)
        {
            switch (state)
            {
                case JxbStateEnum.Chosen:
                    return new SolidColorBrush(ThemeHelper.StringToColor("#4CAF50"));//绿色
                case JxbStateEnum.Error:
                    return new SolidColorBrush(ThemeHelper.StringToColor("#F44336"));//红色
                case JxbStateEnum.NotFound:
                    return new SolidColorBrush(ThemeHelper.StringToColor("#FF5722"));//橙色
                case JxbStateEnum.Ready:
                    return new SolidColorBrush(ThemeHelper.StringToColor("#2196F3"));//蓝色
                case JxbStateEnum.Unknow:
                    return new SolidColorBrush(ThemeHelper.StringToColor("#607D8B"));//灰色
                case JxbStateEnum.Known:
                    return new SolidColorBrush(ThemeHelper.StringToColor("#673AB7"));//紫色
            }
            return new SolidColorBrush(Colors.Black);
        }
    }
}
