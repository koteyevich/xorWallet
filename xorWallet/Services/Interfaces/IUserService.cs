using MongoDB.Bson;
using xorWallet.Models;

namespace xorWallet.Services.Interfaces;

public interface IUserService
{
    IEnumerable<UserModel> GetAllUsers();

    UserModel? GetUserById(ObjectId id);
    UserModel? GetUserById(long id);

    Task AddUser(UserModel user);
    Task EditUser(UserModel user);
    Task DeleteUser(UserModel user);
}