using Newtonsoft.Json;
using System.Text;

namespace SanatoriumQuircoTest.Services.Rooms
{
    internal class RoomsService: IRoomsService
    {
        public RoomsService(string apiUrl)
        {
            _apiUrl = apiUrl;
        }

        private string _apiUrl = String.Empty;
        private string _createRoomEndpoint = "/createRoom";
        private string _inviteUserEndpoint = "/rooms/{roomId}/invite";
        private string _joinEndpoint = "/rooms/{roomId}/join";
        private string _sendMessageEndpoint = "/rooms/{roomId}/send/m.room.message/{txnId}";

        public async Task<string> CreateRoomAsync(string sanatoriumAccountToken)
        {
            using (HttpClient client = new HttpClient())
            {
                var targetUrl = _apiUrl + _createRoomEndpoint;
                string createRoomData = "{\"visibility\": \"private\", \"name\": \"Чат\"}";
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + sanatoriumAccountToken);
                var content = new StringContent(createRoomData, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(targetUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();

                    dynamic roomResponse = JsonConvert.DeserializeObject(jsonResponse);

                    return roomResponse?.room_id; //
                }
                else
                {
                    Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                    return null;
                }
            }
        }

        public async Task<string> InviteUserIntoRoom(string accessToken, string roomId, string inviteeUserId)
        {
            var requestBody = new
            {
                user_id = inviteeUserId
            };

            using (HttpClient client = new HttpClient())
            {
                var targetUrl = _apiUrl + _inviteUserEndpoint;
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

                string inviteUrl = targetUrl.Replace("{roomId}", roomId);

                string requestBodyJson = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(requestBodyJson, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(inviteUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    return $"{inviteeUserId} invited successfully.";
                }
                else
                {
                    return $"Error: {response.StatusCode} - {response.ReasonPhrase}";
                }
            }
        }

        public async Task<string> JoinUserIntoRoom(string accessToken, string roomIdOrAlias)
        {
            using (HttpClient client = new HttpClient())
            {
                var targetUrl = _apiUrl + _joinEndpoint;

                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

                string joinUrl = targetUrl.Replace("{roomIdOrAlias}", roomIdOrAlias);

				var content = new StringContent(String.Empty, Encoding.UTF8, "application/json");

				var response = await client.PostAsync(joinUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    return "Successfully joined the room.";
                }
                else
                {
                    return $"Error: {response.StatusCode} - {response.ReasonPhrase}";
                }
            }
        }

        // TODO:
        public async Task<string> SendHelloFromSanatorium(string accessToken, string roomId, string messageText)
        {
            string transactionId = Guid.NewGuid().ToString();

            var targetUrl = _apiUrl + _sendMessageEndpoint;

            string sendMessageUrl = targetUrl.Replace("{roomId}", roomId).Replace("{txnId}", transactionId);

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

                var messageContent = new
                {
                    msgtype = "m.text",
                    body = messageText
                };

                string messageContentJson = Newtonsoft.Json.JsonConvert.SerializeObject(messageContent);
                var content = new StringContent(messageContentJson, Encoding.UTF8, "application/json");

                var response = await client.PutAsync(sendMessageUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    return $"Message to {roomId} sent successfully.";
                }
                else
                {
                    return $"Error: {response.StatusCode} - {response.ReasonPhrase}";
                }
            }
        }
    }
}
