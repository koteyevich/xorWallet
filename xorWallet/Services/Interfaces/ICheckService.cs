using MongoDB.Bson;
using xorWallet.Models;

namespace xorWallet.Services.Interfaces;

public interface ICheckService
{
    IEnumerable<CheckModel> GetAll();

    CheckModel? Get(ObjectId id);
    CheckModel? Get(string id);
    List<CheckModel> GetAllOfUser(UserModel user);

    Task Add(CheckModel check);
    Task Edit(CheckModel check);
    Task Delete(CheckModel check);
    Task DeleteAllOfUser(UserModel user);
}