using kino.Crypto;
using kino.Database;
using KnmBackend.Controllers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace kino
{
    public class UserViewModel
    {
        public string Id { set; get; }
        public string Username { set; get; }
        public string FullName { set; get; }
        public string Roles { get; set; }

        private UserViewModel()
        {

        }

        public UserViewModel(User user, IEncoder encoder)
        {
            Id = user.Id;
            Username = encoder.Decode(user.UserName);
            FullName = encoder.Decode(user.FullName);
        }

        public UserViewModel(User user, IEncoder encoder, string role)
        {
            Id = user.Id;
            Username = encoder.Decode(user.UserName);
            FullName = encoder.Decode(user.FullName);
            Roles = role;
        }

        public UserViewModel(User user, IEncoder encoder, IEnumerable<string> roles)
        {
            Id = user.Id;
            Username = encoder.Decode(user.UserName);
            FullName = encoder.Decode(user.FullName);
            Roles = AuthController.GetMaximalRole(roles.ToList());
        }
    }
}