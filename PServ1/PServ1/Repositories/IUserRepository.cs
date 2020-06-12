using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PServ1.Repositories
{
    public interface IUserRepository
    {
        int GetUserCount(string userCountId);
    }
}
