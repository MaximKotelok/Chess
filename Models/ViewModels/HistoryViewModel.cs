using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels
{
    public class HistoryViewModel
    {
        public string userId { get; set; }
        public List<Session> history { get; set; }
    }
}
