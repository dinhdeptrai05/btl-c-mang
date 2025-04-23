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
                case CommandType.LoginResponse:
                    {
                        bool success = msg.ReadBoolean();
                        if (success)
                        {
                            int userId = msg.ReadInt();
                            string username = msg.ReadUTF();
                            Console.WriteLine($"Đăng nhập thành công. User ID: {userId}, Username: {username}");
                        }
                        else
                        {
                            string errorMessage = msg.ReadUTF();
                            Console.WriteLine($"Đăng nhập thất bại: {errorMessage}");
                        }
                        break;
                    }
                default:
                    Console.WriteLine("Not found handler for command ID: " + command);
                    break;
            }
        }
    }
}
