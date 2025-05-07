using System.Collections.Generic;

namespace AuctionServer
{
    public class Room
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int OwnerId { get; set; }
        public bool isOpen { get; set; }
        public string TimeCreated { get; set; }
        public List<Item> Items { get; set; }
        public List<Chat> Chats { get; set; }

        public static List<User> Users { get; set; } = new List<User>();

        public Room() { }

        public Room(int id, string name, int ownerId, bool isOpen)
        {
            Id = id;
            Name = name;
            OwnerId = ownerId;
            this.isOpen = isOpen;
            Items = new List<Item>();
        }

        public Room(int id, string name, int ownerId, bool isOpen, List<Item> items, List<Chat> chats)
        {
            Id = id;
            Name = name;
            OwnerId = ownerId;
            this.isOpen = isOpen;
            Items = items;
            Chats = chats;
        }
    }
}
