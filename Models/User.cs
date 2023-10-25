﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Models
{
    public class User : IdentityUser
	{        
        
        public int Reputation { get; set; }
        public List<UserFriend> SendedUserFriends { get; set; }
        public List<UserFriend> ReceivedUserFriends { get; set; }
        public List<Session> SessionsAsWhite { get; set; }        
        public List<Session> SessionsAsBlack { get; set; }


    }
}