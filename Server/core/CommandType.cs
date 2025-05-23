namespace Server.Core
{
    public enum CommandType : sbyte
    {
        Login = 1,
        LoginResponse = 2,
        Register = 3,
        RegisterResponse = 4,
        getAllRoom = 5,
        getAllRoomResponse = 6,
        JoinRoom = 7,
        JoinRoomResponse = 8,
        SendChatMessage = 9,
        ChatMessageReceived = 10,
        LeaveRoom = 11,
        CreateRoom = 12,
        CreateRoomResponse = 13,
        Logout = 14,
        UpdateProfile = 15,
        UpdateProfileResponse = 16,
        PlaceBid = 17,
        PlaceBidResponse = 18,
        UserJoinRoom = 19,
        UserLeaveRoom = 20,
        UpdateTimeLeft = 21,
        AuctionEnd = 22,
        RoomClosed = 23,
        KickedFromRoom = 24
    }
}