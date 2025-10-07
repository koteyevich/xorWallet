using MongoDB.Bson;
using xorWallet.Models;

namespace xorWallet.Services.Interfaces;

public interface IUserService
{
    IEnumerable<UserModel> GetAll();

    UserModel? Get(ObjectId id);
    UserModel? Get(long id);

    Task Add(UserModel user);
    Task Edit(UserModel user);
    Task Delete(UserModel user);
}