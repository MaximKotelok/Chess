using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels
{
    public enum RequestState
    {
        FRIENDS,
        SENDED, 
        NOTHING,
        DENIED
    }
    public class ProfileViewModel
    {
        public RequestState State { get; set; }
        public User User { get; set; }  
    }
}
