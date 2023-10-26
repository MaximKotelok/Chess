using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace Models
{
    public class User : IdentityUser
	{
        public string AvatarPath { get; set; }
        
        public int Reputation { get; set; }
        [JsonIgnore]
        public List<UserFriend> SendedUserFriends { get; set; }
        [JsonIgnore]
        public List<UserFriend> ReceivedUserFriends { get; set; }
        [JsonIgnore]
        public List<Session> SessionsAsWhite { get; set; }
        [JsonIgnore]
        public List<Session> SessionsAsBlack { get; set; }


    }
}