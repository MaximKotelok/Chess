namespace Models
{
    public class UserFriend
    {
        public String SenderUserId { get; set; }
        public User SenderUser { get; set; }

        public String ReceiverUserId { get; set; }
        public User ReceiverUser { get; set; }
        public bool? IsReceived { get; set; } 


    }
}