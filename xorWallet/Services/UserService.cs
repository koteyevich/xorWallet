using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using xorWallet.Models;
using xorWallet.Services.Interfaces;

namespace xorWallet.Services;

public class UserService(DatabaseContext context, ILogger<UserService> logger) : IUserService
{
    public IEnumerable<UserModel> GetAll()
    {
        return context.Users.OrderByDescending(c => c.Id).Take(20).AsNoTracking().AsEnumerable();
    }

    public UserModel? Get(ObjectId id)
    {
        return context.Users.FirstOrDefault(c => c.Id == id);
    }

    public UserModel? Get(long id)
    {
        return context.Users.FirstOrDefault(c => c.UserId == id);
    }

    public async Task Add(UserModel user)
    {
        context.Users.Add(user);

        context.ChangeTracker.DetectChanges();
        logger.LogDebug("{ChangeTracker.DebugView.LongView}", context.ChangeTracker.DebugView.LongView);

        await context.SaveChangesAsync();
        logger.LogInformation("User added.");
    }

    public async Task Edit(UserModel user)
    {
        var userToUpdate = context.Users.FirstOrDefault(c => c.Id == user.Id);

        if (userToUpdate != null)
        {
            userToUpdate.Balance = user.Balance;

            context.Users.Update(userToUpdate);

            context.ChangeTracker.DetectChanges();
            logger.LogDebug("{ChangeTracker.DebugView.LongView}", context.ChangeTracker.DebugView.LongView);

            await context.SaveChangesAsync();
            logger.LogInformation("User updated.");
        }
        else
        {
            throw new ArgumentException("User not found");
        }
    }

    public async Task Delete(UserModel user)
    {
        var userToDelete = context.Users.FirstOrDefault(c => c.Id == user.Id);

        if (userToDelete != null)
        {
            context.Users.Remove(userToDelete);

            context.ChangeTracker.DetectChanges();
            logger.LogDebug("{ChangeTracker.DebugView}", context.ChangeTracker.DebugView.LongView);

            await context.SaveChangesAsync();
            logger.LogInformation("User removed.");
        }
        else
        {
            throw new ArgumentException("User not found");
        }
    }
}