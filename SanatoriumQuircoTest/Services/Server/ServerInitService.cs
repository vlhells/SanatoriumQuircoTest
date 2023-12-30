using Microsoft.Extensions.Logging;
using SanatoriumQuircoTest.Services.Rooms;
using SanatoriumQuircoTest.Services.Users;

namespace SanatoriumQuircoTest.Services
{
    internal class ServerInitService: IServerInitService
    {
        private IUsersService _usersService;
        private IRoomsService _roomsService;
        private ILogger _logger;
        private string _apiUrl;

        public ServerInitService(string apiUrl, IUsersService usersService, IRoomsService roomsService, 
            ILogger logger)
        {
            _apiUrl = apiUrl;
            _usersService = usersService;
            _roomsService = roomsService;
            _logger = logger;
        }

        public async Task InitServer(int numOfGuests, int numOfEmployees, bool refreshToken)
        {
            var guestsTokensAndIds = await CreateAccountsAsync(numOfGuests, "Guest_", "gpass_", refreshToken);
            var empsTokensAndIds = await CreateAccountsAsync(numOfEmployees, "emp_", "epass_", refreshToken);

            var sanatoriumTokenAndId = await _usersService.RegisterUserAccountAsync
                ("Sanatorium", "Quirco", refreshToken); // TODO: Нужно вынести в конфиги.
            var sanatoriumAccessToken = sanatoriumTokenAndId.accessToken;

            await InitRoomsWithAccountsAsync(guestsTokensAndIds, empsTokensAndIds, sanatoriumAccessToken);

            _logger.LogInformation("Successfully inited server.");
        }

        private async Task<List<(string accToken, string id)>> CreateAccountsAsync(int numOfAccounts, string namePrefix, 
            string passPrefix, bool refreshToken)
        {
            List<(string accToken, string id)> accTokensAndIds = new List<(string, string)>();

            for (int i = 0; i <= numOfAccounts; i++)
            {
                var username = namePrefix + i;
                var password = passPrefix + i;
                var tokenAndId = await _usersService.RegisterUserAccountAsync(username, password, refreshToken);

                if (tokenAndId.id.Contains("guest_") || tokenAndId.id.Contains("emp_")) // Check that it is not status-code.
                {
                    accTokensAndIds.Add(tokenAndId);
                    _logger.LogInformation($"Successfully registered {tokenAndId.id}");
                }
                else
                {
                    _logger.LogCritical($"{tokenAndId.accessToken} - {tokenAndId.id}; i: {i}");
                }

                Thread.Sleep(4500); // Эмпирически установленный delay.
            }

            return accTokensAndIds;
        }

        private async Task InitRoomsWithAccountsAsync(List<(string accToken, string id)> guestsTokensAndIds,
            List<(string accToken, string id)> empsTokensAndIds, string sanatoriumAccessToken)
        {
            Random r = new Random();

            foreach (var acc in guestsTokensAndIds)
            {
                var roomId = await _roomsService.CreateRoomAsync(sanatoriumAccessToken);

                var resultOfGuestInvite = await _roomsService.InviteUserIntoRoom(sanatoriumAccessToken, roomId, acc.id);
                var resultOfGuestJoin = await _roomsService.JoinUserIntoRoom(acc.accToken, roomId, _apiUrl);

                CheckResult(resultOfGuestInvite);
                CheckResult(resultOfGuestJoin);

                for (int i = 0; i < 2; i++)
                {
                    int empNum = r.Next(0, empsTokensAndIds.Count() - 1);
                    var resultOfEmpInvite = await _roomsService.InviteUserIntoRoom(sanatoriumAccessToken, roomId, empsTokensAndIds[empNum].id);
                    var resultOfEmpJoin = await _roomsService.JoinUserIntoRoom(empsTokensAndIds[empNum].accToken, roomId, _apiUrl);

                    CheckResult(resultOfEmpInvite);
                    CheckResult(resultOfEmpJoin);
                }

                var resultOfSend = await _roomsService.SendHelloFromSanatorium
                    (sanatoriumAccessToken, roomId, "Добро пожаловать!");

                CheckResult(resultOfSend);
            }
        }

        private void CheckResult(string resultVariable)
        {
            if (resultVariable.Contains("Error: "))
            {
                _logger.LogCritical(resultVariable);
            }
            else
            {
                _logger.LogInformation(resultVariable);
            }
        }
	}
}
