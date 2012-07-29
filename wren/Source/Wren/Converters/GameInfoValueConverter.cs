using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using Wren.Core.GameLibrary;
using System.Globalization;
using Wren.Core;

namespace Wren
{
    class GameInfoValueConverter : IValueConverter
    {        
        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            var game = value as GameInfo;
            if (game != null)
            {
                return game.GetValue(parameter as String);
            }

            return String.Empty;
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
