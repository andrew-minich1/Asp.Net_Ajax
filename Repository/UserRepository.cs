using ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Repository
{
    public class UserRepository : IRepository
    {
        public DbContext Context { get; private set; }

        public UserRepository(DbContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("entitiesContext");
            }
            Context = context;
        }

        public User GetByLogin(string login)
        {
            return Context.Set<User>().Include(u => u.Roles).FirstOrDefault(u => u.Login == login);
        }

        public void Add(User user)
        {
            Context.Set<User>().Add(user);
        }
    }
}
