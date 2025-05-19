using System.Collections.Generic;
using Client.Core;

namespace Client.Model
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
        public AuctionClient Session { get; set; }
        public static List<User> Users = new List<User>();

        public User(int id, string username, string password, string name, string avatar)
        {
            Id = id;
            Username = username;
            Password = password;
            Name = name;
            Avatar = avatar;
        }

        public static User GetUserById(int id)
        {
            return Users.Find(user => user.Id == id);
        }
    }
}
