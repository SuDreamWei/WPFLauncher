using System;
using System.Globalization;
using System.Windows.Data;

namespace WPFLauncher.Converters;

public class BooleanToLoginStatusConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is bool)
		{
			if (!(bool)value)
			{
				return "未登录";
			}
			return "已登录";
		}
		return "未知状态";
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
