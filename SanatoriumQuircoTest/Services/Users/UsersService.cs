using System.Text;
using System.Text.Json;

namespace SanatoriumQuircoTest.Services.Users
{
    internal class UsersService: IUsersService
    {
		private string _baseUrl;

		public UsersService(string serverUrl)
		{
			_baseUrl = serverUrl;
		}

		public async Task CreateUserAsync(string username, string password)
		{
			using (HttpClient client = new HttpClient())
			{
				string baseUrl = _baseUrl;
				string registerEndpoint = "/register";

				string url = $"{baseUrl}{registerEndpoint}";

				string jsonBody = $"{{\"username\": \"{username}\", \"password\": \"{password}\", \"auth\": {{\"type\": \"m.login.dummy\"}}}}";

				HttpResponseMessage response = await client.PostAsync(url, new StringContent(jsonBody, Encoding.UTF8, "application/json"));

				if (response.IsSuccessStatusCode)
				{
					Console.WriteLine("Пользователь успешно создан!");
				}
				else
				{
					Console.WriteLine($"Ошибка: {response.StatusCode} - {response.ReasonPhrase}");
				}
			}
		}

		public async Task<string[]> GetPartOfUsernamesAsync(string adminToken, string prefix)
		{
			using (HttpClient client = new HttpClient())
			{
				string getUsersEndpoint = $"{_baseUrl}/_synapse/admin/v1/users";
				string url = $"{_baseUrl}/_synapse/admin/v1/users";

				client.DefaultRequestHeaders.Add("Authorization", $"Bearer {adminToken}");
				HttpResponseMessage response = await client.GetAsync(url);

				if (response.IsSuccessStatusCode)
				{
					string responseBody = await response.Content.ReadAsStringAsync();

					var usersResponse = JsonSerializer.Deserialize<UsersResponse>(responseBody);

					var guestUsernames = usersResponse?.Users?.Where(u => u.StartsWith($"{prefix}_")).ToArray();

					return guestUsernames ?? Array.Empty<string>();
				}
				else
				{
					Console.WriteLine($"Ошибка при получении списка пользователей: {response.StatusCode} - {response.ReasonPhrase}");
					return Array.Empty<string>();
				}
			}
		}
	}

	public class UsersResponse
	{
		public string[]? Users { get; set; }
	}
}
