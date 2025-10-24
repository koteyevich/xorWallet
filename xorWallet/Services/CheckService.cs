using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using xorWallet.Models;
using xorWallet.Services.Interfaces;

namespace xorWallet.Services;

public class CheckService(DatabaseContext context, ILogger<CheckService> logger) : ICheckService
{
    public IEnumerable<CheckModel> GetAll()
    {
        return context.Checks.OrderByDescending(c => c.Id).Take(20).AsNoTracking().AsEnumerable();
    }

    public CheckModel? Get(ObjectId id)
    {
        return context.Checks.FirstOrDefault(c => c.Id == id);
    }

    public CheckModel? Get(string id)
    {
        return context.Checks.FirstOrDefault(c => c.CheckId == id);
    }

    public List<CheckModel> GetAllOfUser(UserModel user)
    {
        return context.Checks.Where(c => c.OwnerUserId == user.UserId).ToList();
    }

    public async Task Add(CheckModel check)
    {
        context.Checks.Add(check);

        await context.SaveChangesAsync();
        logger.LogInformation("Check added.");
    }

    public async Task Edit(CheckModel check)
    {
        var checkToUpdate = Get(check.Id);

        if (checkToUpdate != null)
        {
            checkToUpdate.Activations = check.Activations;
            checkToUpdate.UsersActivated = check.UsersActivated;

            context.Checks.Update(checkToUpdate);

            await context.SaveChangesAsync();
            logger.LogInformation("Check updated.");
        }
        else
        {
            throw new ArgumentException("Check not found");
        }
    }

    public async Task Delete(CheckModel check)
    {
        var checkToDelete = Get(check.Id);

        if (checkToDelete != null)
        {
            context.Checks.Remove(checkToDelete);

            await context.SaveChangesAsync();
            logger.LogInformation("Check removed.");
        }
        else
        {
            throw new ArgumentException("Check not found");
        }
    }
}