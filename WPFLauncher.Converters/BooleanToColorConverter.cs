using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace WPFLauncher.Converters;

public class BooleanToColorConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is bool)
		{
			if (!(bool)value)
			{
				return new SolidColorBrush(Colors.Red);
			}
			return new SolidColorBrush(Colors.Green);
		}
		return new SolidColorBrush(Colors.Gray);
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
