using MongoDB.Bson;
using xorWallet.Models;

namespace xorWallet.Services.Interfaces;

public interface ICheckService
{
    IEnumerable<CheckModel> GetAllChecks();

    CheckModel? GetCheckById(ObjectId id);
    CheckModel? GetCheckById(string id);
    List<CheckModel> GetChecksOfUser(UserModel user);

    Task AddCheck(CheckModel check);
    Task EditCheck(CheckModel check);
    Task DeleteCheck(CheckModel check);
}