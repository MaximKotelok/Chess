using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels
{
    public class UserFriendViewModel
    {
        public String UserId { get; set; }
        public List<User> Friends { get; set; }

		public List<User> Requests { get; set; }

	}
}
