﻿// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Globalization;

namespace Messenger.Windows
{
	[ValueConversion(typeof(string[]), typeof(Visibility))]
	[ValueConversion(typeof(string), typeof(Visibility))]
	[ValueConversion(typeof(bool), typeof(Visibility))]
	class InverseVisibilityConverter
		: IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Visibility visibility = (Visibility)new VisibilityConverter().Convert(value, targetType, parameter, culture);

			if (visibility == Visibility.Visible)
				return Visibility.Collapsed;
			return Visibility.Visible;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
