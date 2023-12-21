namespace SanatoriumQuircoTest.Services
{
    internal interface IServerInitService
    {
		/// <summary>
		/// Creates given number of users specified by prefix type. prefix in test-task: "Guest", "emp".
		/// </summary>
		Task InitUsersAsync(string prefix, int numOfThisTypeUsers);

		Task CreateRoomsWithGuestsAsync(string adminToken);

		/// <summary>
		/// Add 2 emps into every room. And in case of success sends hello-message (on every step).
		/// </summary>
		Task AddEmployeesIntoRoomsAsync(string adminToken, string helloMessage);
	}
}
