using System.Collections.Generic;
using Newtonsoft.Json;

namespace AuctionServer
{
    public class Room
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int OwnerId { get; set; }
        public bool isOpen { get; set; }
        public int MinParticipant { get; set; }
        public string TimeCreated { get; set; }
        public List<Item> Items { get; set; }
        public List<Chat> Chats { get; set; }
        public List<User> Users { get; set; } = new List<User>();
        public static List<Room> Rooms { get; set; } = new List<Room>();

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

        public Room(int id, string name, int ownerId, int minParticipant, string timeCreated, string items, bool IsOpen, string chats)
        {
            Id = id;
            Name = name;
            OwnerId = ownerId;
            MinParticipant = minParticipant;
            TimeCreated = timeCreated;
            Items = JsonConvert.DeserializeObject<List<Item>>(items);
            Chats = string.IsNullOrEmpty(chats) ? new List<Chat>() : JsonConvert.DeserializeObject<List<Chat>>(chats);
            isOpen = IsOpen;
        }

        public static Room GetRoom(int id)
        {
            return Rooms.FirstOrDefault(r => r.Id == id);
        }
    }
}
