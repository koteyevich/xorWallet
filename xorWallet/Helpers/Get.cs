using Telegram.Bot.Types;
using xorWallet.Models;
using xorWallet.Services;
using xorWallet.Services.Interfaces;

namespace xorWallet.Helpers;

public class Get
{
    private readonly IUserService _users;
    private readonly ICheckService _checks;
    private readonly IInvoiceService _invoices;

    public Get(IUserService users, ICheckService checks, IInvoiceService invoices)
    {
        _users = users;
        _checks = checks;
        _invoices = invoices;
    }

    public async Task<UserModel> User(long id)
    {
        var user = _users.Get(id);
        if (user == null)
        {
            user = new UserModel { UserId = id };

            await _users.Add(user);

            user = _users.Get(id);
            if (user == null)
            {
                throw new InvalidOperationException("Failed to get and/or create user.");
            }
        }

        return user;
    }

    public async Task<CheckModel?> Check(string id)
    {
        var check = _checks.Get(id);
        return check; // do not create one.
    }
}