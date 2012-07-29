using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Wren.Core.GameLibrary;
using Wren.Core;

namespace Wren
{
    public class GameInfoComparer : IComparer
    {
        SortingOptions _sorting;

        public GameInfoComparer(SortingOptions sorting)
        {
            _sorting = sorting;
        }

        public int Compare(object x, object y)
        {
            var game1 = x as GameInfo;
            var game2 = y as GameInfo;

            if (x == null)
                return -1;

            if (y == null)
                return 1;

            if (_sorting == SortingOptions.Year)
            {
                if (((String)game1.GetValue("Year")).CompareTo((String)game2.GetValue("Year")) == -1)
                    return -1;

                if (((String)game1.GetValue("Year")).CompareTo((String)game2.GetValue("Year")) == 1)
                    return 1;

                return ((String)game1.GetValue("Name")).CompareTo((String)game2.GetValue("Name"));
            }

            if (_sorting == SortingOptions.Publisher)
            {
                if (((String)game1.GetValue("Publisher")).CompareTo((String)game2.GetValue("Publisher")) == -1)
                    return -1;

                if (((String)game1.GetValue("Publisher")).CompareTo((String)game2.GetValue("Publisher")) == 1)
                    return 1;

                return ((String)game1.GetValue("Name")).CompareTo((String)game2.GetValue("Name"));
            }

            if (_sorting == SortingOptions.Name)
            {
                return ((String)game1.GetValue("Name")).CompareTo((String)game2.GetValue("Name"));
            }

            return 0;
        }
    }
}
