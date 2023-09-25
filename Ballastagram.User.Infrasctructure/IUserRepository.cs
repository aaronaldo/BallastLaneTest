using Ballastagram.User.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ballastagram.User.Infrasctructure
{
    public interface IUserRepository
    {
        Task<UserModel> GetUser(UserPK pk);
    }
}
