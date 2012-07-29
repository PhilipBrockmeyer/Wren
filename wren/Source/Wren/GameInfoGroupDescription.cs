using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Wren.Core.GameLibrary;
using Wren.Core;

namespace Wren
{
    public class GameInfoGroupDescription : GroupDescription
    {
        SortingOptions _sorting;

        public GameInfoGroupDescription(SortingOptions sorting)
        {
            _sorting = sorting;
        }

        public override Object GroupNameFromItem(object item, int level, System.Globalization.CultureInfo culture)
        {
            var game = item as GameInfo;

            switch (_sorting)
            {
                case SortingOptions.Name:
                    var firstLetter = ((String)game.GetValue("Name")).ToUpper()[0];
                    if ("01234567890".Contains(firstLetter))
                    {
                        return ("123");
                    }

                    return firstLetter;
                case SortingOptions.Year:
                    return ((String)game.GetValue("Year"));
                case SortingOptions.Publisher:
                    return ((String)game.GetValue("Publisher"));
                default:
                    break;
            }

            return String.Empty;
        }
    }
}
