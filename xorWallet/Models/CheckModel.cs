using MongoDB.Bson;
using MongoDB.EntityFrameworkCore;
using xorWallet.Helpers;

namespace xorWallet.Models;

[Collection("Checks")]
public class CheckModel
{
    public ObjectId Id { get; set; }
    public string CheckId { get; set; } = $"{(int)Bank.Check}_{IdGenerator.Generate()}";
    public long OwnerUserId { get; set; }
    public long XORs { get; set; }
    public long Activations { get; set; }
    public List<UserModel> UsersActivated { get; set; } = [];
}