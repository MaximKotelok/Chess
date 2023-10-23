using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class User
    {        
        public String? Id { get; set; }

        public String? Username { get; set; }
        
        public String? Password { get; set; }
        public int Reputation { get; set; }
        public List<UserFriend> FirstUserFriends { get; set; }
        public List<UserFriend> SecondUserFriends { get; set; }
        public List<Session> FirstSessions { get; set; }        
        public List<Session> SecondSessions { get; set; }


        public override string ToString()
        {
            return $"{Id} {Username} {Password}";
        }

    }
}