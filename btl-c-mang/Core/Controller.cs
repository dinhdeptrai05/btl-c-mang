using System;
using Client.enums;
using Client.Forms;
using Client.Forms.Login;
using Client.Forms.Register;

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
                        FormRegister.gI().HandleRegisterResponse(msg);
                        break;
                    }
                case CommandType.getAllRoomsResponse:
                    {
                        FormLobby.gI().HandleLoadRoomsResponse(msg);
                        break;
                    }
                default:
                    Console.WriteLine("Not found handler for command ID: " + command);
                    break;
            }
        }
    }
}
