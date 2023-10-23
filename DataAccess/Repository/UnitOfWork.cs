using DataAccess.Data;
using DataAccess.Repository.IRepository;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _db;

        public IUserRepository? User { get; private set; }

        public IUserFriendRepository? UserFriend { get; private set; }

        public ISessionRepository? Session { get; private set; }

        public UnitOfWork(ApplicationDbContext db)
        {
            this._db = db;
            User = new UserRepository(db);
            UserFriend = new UserFriendRepository(db);
            Session = new SessionRepository(db);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
   }
