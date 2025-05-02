namespace Client.Model
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Item(int id, string name, int ownerId, int price, int roomId, int isOpen)
        {
            Id = id;
            Name = name;
        }
    }
}
