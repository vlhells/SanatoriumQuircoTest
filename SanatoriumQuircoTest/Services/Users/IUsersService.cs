namespace SanatoriumQuircoTest.Services.Users
{
	/// <summary>
	/// The interface determines service for work with usersAPI - register and login to get access token.
	/// </summary>
	internal interface IUsersService
    {
		/// <summary>
		/// Returns a <see cref="string"/> user_id or string with response error status code with reason phrase.
		/// by specifying a <paramref name="username"/> and <paramref name="password"/>.
		/// </summary>
		/// <param name="username">Desired username of account.</param>
		/// <param name="password">Desired password of account.</param>
		/// <returns>The <see cref="string"/> user_id or string w/ response status code and reason phrase.</returns>
		public Task<string> RegisterUserAccountAsync(string username, string password, bool refreshToken);

		/// <summary>
		/// Returns a <see cref="string"/> access_token or string with response error status code
		/// with reason phrase.
		/// by specifying a <paramref name="userId"/> and <paramref name="password"/>.
		/// </summary>
		/// <param name="userId">Id of user.</param>
		/// <param name="password">Users' account password.</param>
		/// <returns>The <see cref="string"/> access_token or string w/ response status code and reason phrase.</returns>
		public Task<string> LoginAsyncAndGetAccToken(string userId, string password);
    }
}
