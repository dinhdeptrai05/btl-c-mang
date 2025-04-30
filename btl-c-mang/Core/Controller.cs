using System;
using Client.enums;
using Client.Forms;

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
                        FormLogin.gI().HandleLoginResponse(msg);
                        break;
                    }
                case CommandType.RegisterResponse:
                    {
                        FormLogin.gI().HandleRegisterResponse(msg);
                        break;
                    }
                default:
                    Console.WriteLine("Not found handler for command ID: " + command);
                    break;
            }
        }
    }
}
