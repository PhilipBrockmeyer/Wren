using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using Wren.Core.Achievements;
using System.Drawing;

namespace Wren.Converters
{
    public class AchievementTitleColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var achievement = value as Achievement;

            if (achievement == null)
                return Color.Gray;

            if (achievement.IsUnlocked)
                return Color.White;

            return Color.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
