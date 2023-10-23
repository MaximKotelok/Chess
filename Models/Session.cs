using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Session
    {
        public String? Id { get; set; }
        //FirstUser
        public User White { get; set; }
        //SecondUser
        public User Black { get; set; }
        public String WhiteId { get; set; }
        public String BlackId { get; set; }
        public String Steps { get; set; }
        public bool? IsWhiteWin { get; set; }

    }
}
