using System;
using Client.enums;

namespace Client.Core
{
    public class Controller
    {
        public static void HandleMessage(Message msg)
        {
            sbyte command = msg.ReadSByte();

            switch (command)
            {
                case 2:
                    {
                        int res = msg.ReadInt();
                        Console.WriteLine("Kết quả: " + res);
                        break;
                    }
                default:
                    Console.WriteLine("Not found handler for command ID: " + command);
                    break;
            }
        }
    }
}
