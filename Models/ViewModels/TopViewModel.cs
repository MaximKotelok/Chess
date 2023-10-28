using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels
{
	public class TopViewModel
	{
        public List<User> Top { get; set; }
        public User CurrentPlayer { get; set; }
    }
}
