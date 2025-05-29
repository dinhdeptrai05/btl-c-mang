namespace Client.Model
{
    public class Chat
    {
        public string time;
        public string name;
        public string message;
        public Chat() { }

        public Chat(string time, string name, string message)
        {
            this.time = time;
            this.name = name;
            this.message = message;
        }
    }
}
