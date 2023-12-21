using System.Text;
using System.Text.Json;

namespace SanatoriumQuircoTest.Services.Rooms
{
    internal class RoomsService: IRoomsService
    {
		private string _baseUrl;

		public RoomsService(string serverUrl)
		{
			_baseUrl = serverUrl;
		}

		public async Task<string> CreateRoomAsync(string accessToken, string roomName)
		{
			using (HttpClient client = new HttpClient())
			{
				string baseUrl = _baseUrl;
				string createRoomEndpoint = "/createRoom";
				//string createRoomEndpoint = "/_matrix/client/v3/createRoom";

				string url = $"{baseUrl}{createRoomEndpoint}";

				string jsonBody = $"{{\"name\": \"{roomName}\", \"visibility\": \"private\"}}";

				client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
				HttpResponseMessage response = await client.PostAsync(url, new StringContent(jsonBody, Encoding.UTF8, "application/json"));

				if (response.IsSuccessStatusCode)
				{
					Console.WriteLine($"Комната '{roomName}' успешно создана!");
					string responseBody = await response.Content.ReadAsStringAsync();
					return responseBody; // id созданной комнаты.
				}
				else
				{
					Console.WriteLine($"Ошибка: {response.StatusCode} - {response.ReasonPhrase}");
					return null;
				}
			}
		}

		public async Task AddUserToRoomAsync(string adminToken, string roomId, string userId)
		{
			using (HttpClient client = new HttpClient())
			{
				string baseUrl = _baseUrl;
				string joinEndpoint = $"/rooms/{roomId}/join";

				string url = $"{baseUrl}{joinEndpoint}";

				string jsonBody = $"{{\"user_id\": \"{userId}\"}}";

				client.DefaultRequestHeaders.Add("Authorization", $"Bearer {adminToken}");
				HttpResponseMessage response = await client.PostAsync(url, new StringContent(jsonBody, Encoding.UTF8, "application/json"));

				if (response.IsSuccessStatusCode)
				{
					Console.WriteLine($"Пользователь '{userId}' успешно добавлен в комнату!");
				}
				else
				{
					Console.WriteLine($"Ошибка при добавлении пользователя '{userId}': {response.StatusCode} - {response.ReasonPhrase}");
				}
			}
		}

		public async Task SendMessageToRoomAsync(string adminToken, string roomId, string message)
		{
			using (HttpClient client = new HttpClient())
			{
				string baseUrl = _baseUrl;
				string sendMessageEndpoint = $"/rooms/{roomId}/send/m.room.message";

				string url = $"{baseUrl}{sendMessageEndpoint}";

				string jsonBody = $"{{\"msgtype\": \"m.text\", \"body\": \"{message}\"}}";

				client.DefaultRequestHeaders.Add("Authorization", $"Bearer {adminToken}");
				HttpResponseMessage response = await client.PostAsync(url, new StringContent(jsonBody, Encoding.UTF8, "application/json"));

				if (response.IsSuccessStatusCode)
				{
					Console.WriteLine($"Сообщение успешно отправлено в комнату!");
				}
				else
				{
					Console.WriteLine($"Ошибка при отправке сообщения: {response.StatusCode} - {response.ReasonPhrase}");
				}
			}
		}

		public async Task<string[]> GetAllRoomsAsync(string adminToken)
		{
			using (HttpClient client = new HttpClient())
			{
				string allRoomsEndpoint = $"{_baseUrl}/_synapse/admin/v1/rooms";
				string url = $"{_baseUrl}/_synapse/admin/v1/rooms";

				client.DefaultRequestHeaders.Add("Authorization", $"Bearer {adminToken}");
				HttpResponseMessage response = await client.GetAsync(url);

				if (response.IsSuccessStatusCode)
				{
					string responseBody = await response.Content.ReadAsStringAsync();
					string[] roomIds = JsonSerializer.Deserialize<string[]>(responseBody);

					return roomIds;
				}
				else
				{
					Console.WriteLine($"Ошибка при получении списка комнат: {response.StatusCode} - {response.ReasonPhrase}");
					return Array.Empty<string>();
				}
			}
		}

	}
}
