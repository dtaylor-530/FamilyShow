using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Data;

namespace Microsoft.FamilyShow
{
  public class Converter : IValueConverter
  {


    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {

      return value;
    }

        public static Converter Instance { get; } = new Converter();
  }
}
