using DataAccess.Data;
using DataAccess.Repository.IRepository;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class UserFriendRepository : Repository<UserFriend>, IUserFriendRepository
    {
        public UserFriendRepository(ApplicationDbContext db) : base(db) { }
        public void Update(UserFriend userFriend)
        {
            _db?.UserFriends?.Update(userFriend);
        }
    }
}
