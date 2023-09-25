using Ballastagram.Commom;

namespace Ballastagram.User.Models
{
    public class UserModel
    {
        public ulong Id { get; set; }
        public string Name { get; set; }
        public string Bio { get; set; }
        public string Email { get; set; }
        public string Nickname { get; set; }
        public DateTime Birthdate { get; set; }
    }

    #region Keys
    public class UserPK : ModelKey<UserModel>
    {
        public ulong Id { get; set; }
    }
    
    #endregion
}
