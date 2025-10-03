using MongoDB.Bson;
using MongoDB.EntityFrameworkCore;

namespace xorWallet.Models;

[Collection("Users")]
public class UserModel
{
    public ObjectId Id { get; set; }
    public long UserId { get; set; }

    public long Balance { get; set; }
}