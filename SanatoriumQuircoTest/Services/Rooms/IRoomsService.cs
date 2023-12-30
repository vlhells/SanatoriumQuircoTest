using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;

namespace SanatoriumQuircoTest.Services.Rooms
{
    internal interface IRoomsService
    {
        public Task<string> CreateRoomAsync(string sanatoriumAccountToken);

        public Task<string> InviteUserIntoRoom(string accessToken, string roomId, string inviteeUserId);

        public Task<string> JoinUserIntoRoom(string accessToken, string roomIdOrAlias, string serverName);

        public Task<string> SendHelloFromSanatorium(string accessToken, string roomId, string messageText);
    }
}
