using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace SanatoriumQuircoTest.Services.Users
{
    internal class UsersService : IUsersService
    {
        public UsersService(string apiUrl)
        {
            _apiUrl = apiUrl;
        }

        private string _apiUrl = String.Empty;
        private string _registerEndpointUrl = "/register";

        private async Task<string> GetSessionFromServerAsync(string username, string password, bool refreshToken)
        {
            using (HttpClient client = new HttpClient())
            {
                var targetUrl = _apiUrl + _registerEndpointUrl;

                var requestParams = new
                {
                    password = password,
                    refresh_token = refreshToken,
                    username = username
                };

                string jsonContent = Newtonsoft.Json.JsonConvert.SerializeObject(requestParams);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(targetUrl, content);

                string responseBody = await response.Content.ReadAsStringAsync();

                dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(responseBody);

                string session = json.session;

                return session;
            }
        }

        private async Task<(string access, string id)> FinalizeRegisterAsync(string username, string password, string sessionKey,
            bool refreshToken)
        {
            using (HttpClient client = new HttpClient())
            {
                var targetUrl = _apiUrl + _registerEndpointUrl;

                var requestParams = new
                {
                    password = password,
                    refresh_token = refreshToken,
                    username = username,
                    auth = new
                    {
                        type = "m.login.dummy",
                        session = sessionKey
                    }
                };

                string jsonBody = Newtonsoft.Json.JsonConvert.SerializeObject(requestParams);

                StringContent content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(targetUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();

                    dynamic tokenResponse = JsonConvert.DeserializeObject(jsonResponse);

                    string accessToken = tokenResponse.access_token;
                    string userId = tokenResponse.user_id;

                    return (accessToken, userId);
                }
                else
                {
                    return (response.StatusCode.ToString(), response.ReasonPhrase);
                }
            }
        }

        public async Task<(string accessToken, string id)> RegisterUserAccountAsync(string username, string password, bool refreshToken)
        {
            var session = await GetSessionFromServerAsync(username, password, refreshToken);
            return await FinalizeRegisterAsync(username, password, session, refreshToken);
        }
    }
}
