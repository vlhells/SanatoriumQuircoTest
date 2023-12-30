using Microsoft.Extensions.Logging;
using SanatoriumQuircoTest.Services;
using SanatoriumQuircoTest.Services.Rooms;
using SanatoriumQuircoTest.Services.Users;

namespace SanatoriumQuircoTest
{
	internal class Program
	{
		static async Task Main()
        {
            // TODO: Test work,
            // XMLs in interfaces and models.
            string apiUrl = "https://matrix.quirco.com/_matrix/client/v3";

            var usersService = new UsersService(apiUrl);
            var roomsService = new RoomsService(apiUrl);
            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });

            ILogger<ServerInitService> logger = loggerFactory.CreateLogger<ServerInitService>();

            var serverInitService = new ServerInitService(apiUrl, usersService, roomsService, logger);

            await serverInitService.InitServer(2000, 50, false);
        }
	}
}