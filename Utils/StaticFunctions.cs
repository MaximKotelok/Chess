using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Utils
{
    public static class StaticFunctions
    {
        public static string CreateUniqueId()
        {
            return Guid.NewGuid().ToString("N");
        }  
        public static bool IsMoveValidRegex(string move)
        {
            string pattern = @"^\d+:\d+-\d+:\d+( \d+:\d+-\d+:\d+)*$";


            return Regex.IsMatch(move, pattern);
        }
    }
}
