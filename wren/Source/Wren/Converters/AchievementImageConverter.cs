using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Globalization;
using Wren.Core.Achievements;
using System.IO;
using System.Windows.Controls;
using Wren.Core;
using System.Threading;
using System.Windows.Threading;

namespace Wren.Converters
{
    public class AchievementImageConverter : IMultiValueConverter
    {
        public Object Convert(Object[] values, Type targetType, Object parameter, CultureInfo culture)
        {
            if (values[1] == null)
                return null;

            Image image = values[0] as Image;
            BitmapImage src = new BitmapImage(new Uri("pack://application:,,,/Wren;component/Images/Image-Missing-64.png", UriKind.Absolute));
            image.Source = src;

            Achievement achievement = values[1] as Achievement;

            String url = String.Empty;
            if (achievement.IsUnlocked)
                url = achievement.UnlockedImage;
            else
                url = achievement.LockedImage;

            ThreadPool.QueueUserWorkItem((WaitCallback)delegate
            {
                Stream imageStream = ImageCache.GetImage(url);
                if (imageStream != null)
                {
                    image.Dispatcher.Invoke(DispatcherPriority.Normal,
                        (ThreadStart)delegate
                        {
                            src = new BitmapImage();
                            src.BeginInit();
                            src.StreamSource = imageStream;
                            src.EndInit();
                            image.Source = src;
                        });
                }
            });           

            return src;            
        }

        public Object[] ConvertBack(Object value, Type[] targetTypes, Object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
