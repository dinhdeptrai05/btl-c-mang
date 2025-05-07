using System.Collections.Generic;

namespace Client.Model
{
    public class Room
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int OwnerId { get; set; }
        public bool isOpen { get; set; }

        public string TimeCreated { get; set; }
        public List<Item> Items { get; set; }

        public Room(int id, string name, int ownerId, bool isOpen)
        {
            Id = id;
            Name = name;
            OwnerId = ownerId;
            this.isOpen = isOpen;
            Items = new List<Item>();
        }

        public Room(int id, string name, int ownerId, bool isOpen, List<Item> items)
        {
            Id = id;
            Name = name;
            OwnerId = ownerId;
            this.isOpen = isOpen;
            Items = items;
        }


    }
}
