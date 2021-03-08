using System;
using System.Collections.Generic;
using System.Text;

namespace LOCCounter
{
    static class Utilities
    {
        public static bool ContainsIgnoreCase(this List<string> list, string item)
        {
            return list.FindIndex(s => s.Equals(item, StringComparison.OrdinalIgnoreCase)) != -1;
        }
    }
}
