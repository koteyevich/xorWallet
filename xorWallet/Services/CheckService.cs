using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using xorWallet.Models;
using xorWallet.Services.Interfaces;

namespace xorWallet.Services;

public class CheckService(DatabaseContext context, ILogger<CheckService> logger) : ICheckService
{
    public IEnumerable<CheckModel> GetAllChecks()
    {
        return context.Checks.OrderByDescending(c => c.Id).Take(20).AsNoTracking().AsEnumerable();
    }

    public CheckModel? GetCheckById(ObjectId id)
    {
        return context.Checks.FirstOrDefault(c => c.Id == id);
    }

    public CheckModel? GetCheckById(string id)
    {
        return context.Checks.FirstOrDefault(c => c.CheckId == id);
    }

    public List<CheckModel> GetChecksOfUser(UserModel user)
    {
        return context.Checks.Where(c => c.OwnerUserId == user.UserId).ToList();
    }

    public async Task AddCheck(CheckModel check)
    {
        context.Checks.Add(check);

        context.ChangeTracker.DetectChanges();
        logger.LogDebug("{ChangeTracker.DebugView.LongView}", context.ChangeTracker.DebugView.LongView);

        await context.SaveChangesAsync();
        logger.LogInformation("Check added.");
    }

    public async Task EditCheck(CheckModel check)
    {
        var checkToUpdate = GetCheckById(check.Id);

        if (checkToUpdate != null)
        {
            checkToUpdate.Activations = check.Activations;
            checkToUpdate.UsersActivated = check.UsersActivated;

            context.Checks.Update(checkToUpdate);

            context.ChangeTracker.DetectChanges();
            logger.LogDebug("{ChangeTracker.DebugView.LongView}", context.ChangeTracker.DebugView.LongView);

            await context.SaveChangesAsync();
            logger.LogInformation("Check updated.");
        }
        else
        {
            throw new ArgumentException("Check not found");
        }
    }

    public async Task DeleteCheck(CheckModel check)
    {
        var checkToDelete = GetCheckById(check.Id);

        if (checkToDelete != null)
        {
            context.Checks.Remove(checkToDelete);

            context.ChangeTracker.DetectChanges();
            logger.LogDebug("{ChangeTracker.DebugView}", context.ChangeTracker.DebugView.LongView);

            await context.SaveChangesAsync();
            logger.LogInformation("Check removed.");
        }
        else
        {
            throw new ArgumentException("Check not found");
        }
    }
}