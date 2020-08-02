using System;
using System.Collections.Generic;
using System.Text;

namespace MusicOrder.Extensions
{
    public static class StringExtensions
    {
        public static bool IsNothing(this string text)
        {
            return string.IsNullOrWhiteSpace(text);
        }
    }
}
