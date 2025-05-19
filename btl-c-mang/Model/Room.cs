using System.Collections.Generic;

namespace Client.Model
{
    public class Room
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int OwnerId { get; set; }
        public string OwnerName { get; set; }
        public bool isOpen { get; set; }
        public string TimeCreated { get; set; }
        public List<Item> Items { get; set; }
        public List<Chat> Chats { get; set; }

        public Room(int id, string name, int ownerId, string ownerName, bool isOpen)
        {
            Id = id;
            Name = name;
            OwnerId = ownerId;
            OwnerName = ownerName;
            this.isOpen = isOpen;
            Items = new List<Item>();
        }

        public Room(int id, string name, int ownerId, string ownerName, bool isOpen, List<Item> items, List<Chat> chats)
        {
            Id = id;
            Name = name;
            OwnerId = ownerId;
            OwnerName = ownerName;
            this.isOpen = isOpen;
            Items = items;
            Chats = chats;
        }
    }

}
