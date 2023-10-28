using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels
{
    public class ReplayViewModel
    {
        public Dictionary<int, string> WhiteMoves { get; set; }
        public Dictionary<int, string> BlackMoves { get; set; }
    }
}
