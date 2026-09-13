using System;
using System.Globalization;
using System.Windows.Data;

namespace WPFLauncher.Converters;

public class BooleanToCurrentSelectionConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is bool)
		{
			if (!(bool)value)
			{
				return "";
			}
			return "当前选择";
		}
		return "";
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
