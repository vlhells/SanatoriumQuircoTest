using Microsoft.Extensions.Logging;
using SanatoriumQuircoTest.Services;
using SanatoriumQuircoTest.Services.Rooms;
using SanatoriumQuircoTest.Services.Users;
using SanatoriumQuircoTest.TxtLogger;

namespace SanatoriumQuircoTest
{
	internal class Program
	{
		static async Task Main()
        {
            string apiUrl = "https://matrix.quirco.com/_matrix/client/v3"; // TODO: Нужно вынести в конфиг.

            var usersService = new UsersService(apiUrl);
            var roomsService = new RoomsService(apiUrl);
            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
				builder.AddFile(Path.Combine(Directory.GetCurrentDirectory(), "log.txt"));
			});

			ILogger<ServerInitService> logger = loggerFactory.CreateLogger<ServerInitService>();

            var serverInitService = new ServerInitService(usersService, roomsService, logger);

            await serverInitService.InitServer(2000, 50, false);
        }
	}
}