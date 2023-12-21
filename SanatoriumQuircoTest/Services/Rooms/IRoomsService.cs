using System.Threading.Tasks;

namespace SanatoriumQuircoTest.Services.Rooms
{
    internal interface IRoomsService
    {
        Task<string> CreateRoomAsync(string accessToken, string roomName);
        Task AddUserToRoomAsync(string adminToken, string roomId, string userId);
        Task SendMessageToRoomAsync(string adminToken, string roomId, string message);
        Task<string[]> GetAllRoomsAsync(string adminToken);
	}
}
