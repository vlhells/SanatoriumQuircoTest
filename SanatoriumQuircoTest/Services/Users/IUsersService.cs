namespace SanatoriumQuircoTest.Services.Users
{
    internal interface IUsersService
    {
        Task CreateUserAsync(string username, string password);

		/// <summary>
		/// Returns part of usernames specified by prefix type. prefix in test-task: "Guest", "emp".
		/// </summary>
		Task<string[]> GetPartOfUsernamesAsync(string adminToken, string prefix);
	}
}
