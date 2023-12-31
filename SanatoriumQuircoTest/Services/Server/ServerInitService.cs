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

		// TODO: Нужно вынести в конфиги:
		private string _sanatoriumAccountLogin = "Sanatorium";
        private string _sanatoriumAccountPassword = "Quirco";
        //.

        private string _guestLoginPrefix = "Guest_";
        private string _empLoginPrefix = "emp_";
        private string _guestPasswordPrefix = "gpass_";
        private string _empPasswordPrefix = "epass_";

        public ServerInitService(IUsersService usersService, IRoomsService roomsService, ILogger logger)
        {
            _usersService = usersService;
            _roomsService = roomsService;
            _logger = logger;
        }

        public async Task InitServer(int numOfGuests, int numOfEmployees, bool refreshToken)
        {
            await CreateAccountsAsync(numOfGuests, _guestLoginPrefix, _guestPasswordPrefix, refreshToken);
            await CreateAccountsAsync(numOfEmployees, _empLoginPrefix, _empPasswordPrefix, refreshToken);

			var sanatoriumId = await _usersService.RegisterUserAccountAsync(_sanatoriumAccountLogin, 
                _sanatoriumAccountPassword, refreshToken);
			if (sanatoriumId.Contains(_sanatoriumAccountLogin.ToLower())) // Check that it is not status-code.
			{
				_logger.LogInformation($"Successfully registered {sanatoriumId}");
			}
			else // log status code and reason (in id variable).
			{
				_logger.LogCritical($"{sanatoriumId}");
			}

            await InitRoomsWithAccountsAsync(numOfGuests, numOfEmployees);

            _logger.LogInformation("Successfully inited server.");
        }

        private async Task CreateAccountsAsync(int numOfAccounts, string namePrefix, 
            string passPrefix, bool refreshToken)
        {
            for (int i = 0; i <= numOfAccounts; i++)
            {
                var username = namePrefix + i;
                var password = passPrefix + i;
                var id = await _usersService.RegisterUserAccountAsync(username, password, refreshToken);

                if (id.Contains(_guestLoginPrefix.ToLower()) || id.Contains(_empLoginPrefix)) // Check that it is not status-code.
                {
                    _logger.LogInformation($"Successfully registered {id}");
                }
                else // log status code and reason (in id variable).
                {
                    _logger.LogCritical($"{id}; i: {i}");
                }

                Thread.Sleep(4500); // Эмпирически установленный delay.
            }
        }

        private async Task InitRoomsWithAccountsAsync(int numOfGuests, int numOfEmps)
        {
            Random r = new Random();

            var sanatoriumAccessToken = await _usersService.LoginAsyncAndGetAccToken
                ($"@{_sanatoriumAccountLogin}:matrix.quirco.com", _sanatoriumAccountPassword);

            for (int i = 0; i < numOfGuests; i++)
            {
                var guestId = $"@{_guestLoginPrefix.ToLower()}{i}:matrix.quirco.com";
                var guestPassword = $"{_guestPasswordPrefix}{i}";

                var guestAccToken = await _usersService.LoginAsyncAndGetAccToken(guestId, guestPassword);

                var roomId = await _roomsService.CreateRoomAsync(sanatoriumAccessToken);

                var resultOfGuestInvite = await _roomsService.InviteUserIntoRoom(sanatoriumAccessToken, roomId, guestId);
                var resultOfGuestJoin = await _roomsService.JoinUserIntoRoom(guestAccToken, roomId);

                CheckResult(resultOfGuestInvite);
                CheckResult(resultOfGuestJoin);

                for (int j = 0; j < 2; j++)
                {
					int empNum;
                    int prevEmpNum = -1; // For init this variable.
                    do
                    {
                        empNum = r.Next(0, numOfEmps);
                    } while (empNum == prevEmpNum);
					var empId = $"@{_empLoginPrefix}{empNum}:matrix.quirco.com";
                    var empPassword = $"{_empPasswordPrefix}{empNum}";
                    var empAccToken = await _usersService.LoginAsyncAndGetAccToken(empId, empPassword);
					var resultOfEmpInvite = await _roomsService.InviteUserIntoRoom(sanatoriumAccessToken, roomId, empId);
                    var resultOfEmpJoin = await _roomsService.JoinUserIntoRoom(empAccToken, roomId);

                    prevEmpNum = empNum;

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
