namespace SanatoriumQuircoTest.Services.Rooms
{
	/// <summary>
	/// The interface determines service for work with roomsAPI - create, invite, join, sendMessage.
	/// </summary>
	internal interface IRoomsService
    {
		/// <summary>
		/// Returns a <see cref="string"/> room_id or string with response error status code
		/// with reason phrase
		/// by specifying an <paramref name="accessToken"/>.
		/// </summary>
		/// <param name="accessToken">Token got in proc of registration or login into user account.</param>
		/// <returns>The <see cref="string"/> room_id or string w/ response status code and reason phrase.</returns>
		public Task<string> CreateRoomAsync(string accessToken);

		/// <summary>
		/// Returns a <see cref="string"/> with successful result or string with response error status code
		/// with reason phrase
		/// by specifying an <paramref name="accessToken"/>, <paramref name="roomId"/>, 
		/// <paramref name="inviteeUserId"/>.
		/// </summary>
		/// <param name="accessToken">Token got in proc of registration or login into user account.</param>
		/// <param name="roomId">Room id where u want invite user.</param>
		/// <param name="inviteeUserId">Id of user u wanna invite.</param>
		/// <returns>The <see cref="string"/> with successful result or string w/ response status code and reason phrase.</returns>
		public Task<string> InviteUserIntoRoomAsync(string accessToken, string roomId, string inviteeUserId);

		/// <summary>
		/// Returns a <see cref="string"/> with successful result or string with response error status code
		/// with reason phrase
		/// by specifying an <paramref name="accessToken"/>, <paramref name="roomIdOrAlias"/>.
		/// </summary>
		/// <param name="accessToken">Token got in proc of registration or login into user account.</param>
		/// <param name="roomIdOrAlias">Room id where u wanna join.</param>
		/// <returns>The <see cref="string"/> with successful result or string w/ response status code and reason phrase.</returns>
		public Task<string> JoinUserIntoRoomAsync(string accessToken, string roomIdOrAlias);

		/// <summary>
		/// Returns a <see cref="string"/> with successful result or string with response error status code
		/// with reason phrase
		/// by specifying an <paramref name="accessToken"/>, <paramref name="roomId"/> and 
		/// <paramref name="messageText"/>.
		/// </summary>
		/// <param name="accessToken">Token got in proc of registration or login into user account.</param>
		/// <param name="roomId">Room id where u want to send message.</param>
		/// <param name="messageText">Text of your message.</param>
		/// <returns>The <see cref="string"/> with successful result or string w/ response status code and reason phrase.</returns>
		public Task<string> SendMessageAsync(string accessToken, string roomId, string messageText);
    }
}
