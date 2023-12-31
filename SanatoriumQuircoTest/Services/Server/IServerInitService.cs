namespace SanatoriumQuircoTest.Services
{
	/// <summary>
	/// The interface determines service for init server - create users and rooms.
	/// </summary>
	internal interface IServerInitService
    {
        /// <summary>
        /// Initializes the server with its' class submethods by specifying 
        /// a <paramref name="numOfGuests"/>, <paramref name="numOfEmployees"/> and <paramref name="refreshToken"/>,
        /// <paramref name="generalUserAvatarUrl"/>
        /// and log submethods action results.
        /// </summary>
        /// <param name="numOfGuests">Integer number of guests of sanatorium (not guests in Matrix-API)
        /// to be registered.</param>
        /// <param name="numOfEmployees">Integer number of employees of sanatorium to be registered.</param>
        /// <param name="refreshToken">Boolean value.</param>
        /// <param name="generalUserAvatarUrl">String with url to picture.</param>
        public Task InitServer(int numOfGuests, int numOfEmployees, bool refreshToken, string generalUserAvatarUrl);
	}
}
