using SanatoriumQuircoTest.Services;
using SanatoriumQuircoTest.Services.Rooms;
using SanatoriumQuircoTest.Services.Users;

namespace SanatoriumQuircoTest
{
	internal class Program
	{
		static void Main()
		{
			// TODO: CancellationTokens.

			string serverUrl = "https://matrix.quirco.com/";
			IUsersService usersService = new UsersService(serverUrl);
			IRoomsService roomsService = new RoomsService(serverUrl);
			IServerInitService serverInitService = new ServerInitService(usersService, roomsService);

			serverInitService.InitUsersAsync("Guest", 2000);
			serverInitService.InitUsersAsync("emp", 50);

			string adminToken; //
			serverInitService.CreateRoomsWithGuestsAsync(adminToken);
			serverInitService.AddEmployeesIntoRoomsAsync(adminToken, "Добро пожаловать в чат!");
		}
	}
}