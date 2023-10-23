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
    public class SessionRepository:Repository<Session>, ISessionRepository
    {
        public SessionRepository(ApplicationDbContext db) : base(db) { }
        public void Update(Session session)
        {
            _db?.Sessions?.Update(session);
        }
    }
}
