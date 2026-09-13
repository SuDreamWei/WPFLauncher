using System;
using System.Globalization;
using System.Windows.Data;

namespace WPFLauncher.Converters;

public class BooleanToStringConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is bool)
		{
			if (!(bool)value)
			{
				return "非活跃";
			}
			return "活跃";
		}
		return "未知";
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
