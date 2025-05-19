namespace AuctionServer
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
        public ClientSession Session { get; set; }

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
          try {
            string query = "SELECT * FROM accounts WHERE id = @param0";
            var result = Database.gI().ExecuteQuery(query, id);
            if (result.Rows.Count > 0)
            {
                return new User(
                    int.Parse(result.Rows[0]["id"].ToString()),
                    result.Rows[0]["username"].ToString(),
                    result.Rows[0]["password"].ToString(),
                    result.Rows[0]["name"].ToString(),
                    result.Rows[0]["avatar_url"].ToString()
                );
            }
            return null;
          }
          catch (Exception ex)
          {
            Console.WriteLine($"Lỗi khi lấy user: {ex}");
            return null;
          }
        }
    }
}
