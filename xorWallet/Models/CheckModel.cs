using MongoDB.Bson;
using MongoDB.EntityFrameworkCore;
using xorWallet.Helpers;

namespace xorWallet.Models;

[Collection("Checks")]
public class CheckModel
{
    public ObjectId Id { get; set; }

    public string CheckId { get; set; } = $"check_{IdGenerator.Generate()}"; /*
                                                                                embedding "check_" is not really necessary,
                                                                                but makes it easier to parse in /start
                                                                            */

    public long OwnerUserId { get; set; }
    public long XORs { get; set; }
    public long Activations { get; set; }
    public List<UserModel> UsersActivated { get; set; } = [];
}