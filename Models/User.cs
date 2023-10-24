using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Models
{
    public class User : IdentityUser
	{        
        
        public int Reputation { get; set; }
        public List<UserFriend> FirstUserFriends { get; set; }
        public List<UserFriend> SecondUserFriends { get; set; }
        public List<Session> FirstSessions { get; set; }        
        public List<Session> SecondSessions { get; set; }


    }
}