using System;
using System.Collections.Generic;

namespace ImdbPopulate.Core
{
    public static class Helper
    {
        public static void Each<T>(this IEnumerable<T> list, Action<T> call)
        {
            foreach (var value in list)
            {
                call(value);
            }
        }
    }
}