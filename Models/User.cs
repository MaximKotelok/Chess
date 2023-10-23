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
        public List<UserFriend> Friends { get; set; }
        public List<Session> Sessions { get; set; }
        
    }
}