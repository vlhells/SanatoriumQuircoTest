using Newtonsoft.Json;
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
        private string _registerEndpoint = "/register";
        private string _loginEndpoint = "/login";
        private string _setAvatarEndpoint = "/profile/{userId}/avatar_url";

        private async Task<string> GetSessionFromServerAsync(string username, string password, bool refreshToken)
        {
            using (HttpClient client = new HttpClient())
            {
                var targetUrl = _apiUrl + _registerEndpoint;

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

        public async Task<string> LoginAsyncAndGetAccToken(string userId, string password)
        {
            using (HttpClient client = new HttpClient())
            {
                var targetUrl = _apiUrl + _loginEndpoint;

                var requestParams = new
                {
                    type = "m.login.password",
                    password = password,
                    identifier = new
                    {
                        type = "m.id.user",
                        user = userId
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

                    return accessToken;
                }
                else
                {
                    return $"{response.StatusCode} - {response.ReasonPhrase}";
                }
            }
        }

        private async Task<(string userId, string accToken)> FinalizeRegisterAsync(string username, 
            string password, string sessionKey,
            bool refreshToken)
        {
            using (HttpClient client = new HttpClient())
            {
                var targetUrl = _apiUrl + _registerEndpoint;

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

                    string userId = tokenResponse.user_id;
                    string accessToken = tokenResponse.access_token;

                    return (userId, accessToken);
                }
                else
                {
                    return (response.StatusCode.ToString(), response.ReasonPhrase);
                }
            }
        }

        public async Task<(string userId, string accToken)> RegisterUserAccountAsync(string username, string password, bool refreshToken)
        {
            var session = await GetSessionFromServerAsync(username, password, refreshToken);
            return await FinalizeRegisterAsync(username, password, session, refreshToken);
        }

        public async Task<string> SetAvatarAsync(string userId, string accessToken, string avatarUrl)
        {
            var requestBody = new
            {
                avatar_url = avatarUrl
            };

            using (HttpClient client = new HttpClient())
            {
                var targetUrl = _apiUrl + _setAvatarEndpoint;
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

                string setAvatarUrl = targetUrl.Replace("{userId}", userId);

                string requestBodyJson = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);

                var content = new StringContent(requestBodyJson, Encoding.UTF8, "application/json");

                var response = await client.PutAsync(setAvatarUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    return $"{userId} avatar set successfully.";
                }
                else
                {
                    return $"Error: {response.StatusCode} - {response.ReasonPhrase}";
                }
            }
        }
    }
}
