using System;
using Client.enums;
using Client.Forms;
using Client.Forms.Login;
using Client.Forms.Register;
using Client.Forms.Lobby;

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
                case CommandType.getAllRoomResponse:
                    {
                        FormLobby.gI().HandleLoadRoomsResponse(msg);
                        break;
                    }
                case CommandType.JoinRoomResponse:
                    {
                        FormLobby.gI().HandleJoinRoomResponse(msg);
                        break;
                    }
                case CommandType.ChatMessageReceived:
                    {
                        FormLobby.gI().HandleChatMessageReceived(msg);
                        break;
                    }
                case CommandType.UserJoinRoom:
                    {
                        FormLobby.gI().HandleChatMessageReceived(msg);
                        break;
                    }
                case CommandType.UserLeaveRoom:
                    {
                        FormLobby.gI().HandleChatMessageReceived(msg);
                        break;
                    }
                case CommandType.CreateRoomResponse:
                    {
                        FormLobby.gI().HandleCreateRoomResponse(msg);
                        break;
                    }
                case CommandType.AddItemResponse:
                    {
                        FormLobby.gI().HandleAddItemResponse(msg);
                        break;
                    }
                case CommandType.StartAuctionResponse:
                    {
                        FormLobby.gI().HandleStartAuctionResponse(msg);
                        break;
                    }
                case CommandType.AuctionStarted:
                    {
                        FormLobby.gI().HandleAuctionStarted(msg);
                        break;
                    }
                case CommandType.BuyNowResponse:
                    {
                        FormLobby.gI().HandleBuyNowResponse(msg);
                        break;
                    }
                case CommandType.UpdateProfileResponse:
                    {
                        FormProfile.gI().HandleUpdateProfileResponse(msg);
                        break;
                    }
                case CommandType.PlaceBidResponse:
                    {
                        FormLobby.gI().HandlePlaceBidResponse(msg);
                        break;
                    }
                default:
                    Console.WriteLine("Not found handler for command ID: " + command);
                    break;
            }
        }
    }
}
