namespace AppLibrary.DataAccess.Interfaces
{
    public interface IUserData
    {
        Task CreateUser(UserModel user);
        Task<UserModel> GetUserByIdAsync(string id);
        Task<UserModel> GetUserFromAuthentication(string objectId);
        Task<List<UserModel>> GetUsersAsync();
        Task UpdateUser(UserModel user);
    }
}