using SanatoriumQuircoTest.Services.Rooms;
using SanatoriumQuircoTest.Services.Users;

namespace SanatoriumQuircoTest.Services
{
    internal class ServerInitService: IServerInitService
    {
		private IUsersService _usersService;
		private IRoomsService _roomsService;

		public ServerInitService(IUsersService usersService, IRoomsService roomsService)
		{
			_usersService = usersService;
			_roomsService = roomsService;
		}

		public async Task InitUsersAsync(string prefix, int numOfThisTypeUsers)
		{
			for (int i = 0; i < numOfThisTypeUsers; i++)
			{
				await _usersService.CreateUserAsync($"{prefix}_{i}", $"{prefix}_{i}");
			}
		}

		public async Task CreateRoomsWithGuestsAsync(string adminToken)
		{
			var guestsNames = await _usersService.GetPartOfUsernamesAsync(adminToken, "Guest");

			for (int i = 0; i < guestsNames.Count(); i++)
			{
				var roomId = await _roomsService.CreateRoomAsync(adminToken, guestsNames[i]);
				await _roomsService.AddUserToRoomAsync(adminToken, roomId, guestsNames[i]);
			}
		}
		
		public async Task AddEmployeesIntoRoomsAsync(string adminToken, string helloMessage)
		{
			Random random = new Random();
			var empNames = await _usersService.GetPartOfUsernamesAsync(adminToken, "emp");
			var rooms = await _roomsService.GetAllRoomsAsync(adminToken);

			for (int i = 0; i < rooms.Count(); i++)
			{
				string emp1 = empNames[random.Next(0, empNames.Count())];
				string emp2;
				do
				{
					emp2 = empNames[random.Next(0, empNames.Count())];
				} while (emp1 == emp2);


				await _roomsService.AddUserToRoomAsync(adminToken, rooms[i], emp1);
				await _roomsService.AddUserToRoomAsync(adminToken, rooms[i], emp2);

				await _roomsService.SendMessageToRoomAsync(adminToken, rooms[i], helloMessage);
			}
		}
	}
}
