﻿namespace SanatoriumQuircoTest.Services.Users
{
    internal interface IUsersService
    {
        // TODO: XML.
        public Task<(string accessToken, string id)> RegisterUserAccountAsync(string username, string password, bool refreshToken);
    }
}
