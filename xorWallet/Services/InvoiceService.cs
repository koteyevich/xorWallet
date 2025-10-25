using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using xorWallet.Models;
using xorWallet.Services.Interfaces;

namespace xorWallet.Services;

public class InvoiceService(DatabaseContext context, ILogger<InvoiceService> logger) : IInvoiceService
{
    public IEnumerable<InvoiceModel> GetAll()
    {
        return context.Invoices.OrderByDescending(c => c.Id).Take(20).AsNoTracking().AsEnumerable();
    }

    public InvoiceModel? Get(ObjectId id)
    {
        return context.Invoices.FirstOrDefault(c => c.Id == id);
    }

    public InvoiceModel? Get(string id)
    {
        return context.Invoices.FirstOrDefault(c => c.InvoiceId == id);
    }

    public List<InvoiceModel> GetAllOfUser(UserModel user)
    {
        return context.Invoices.Where(i => i.OwnerUserId == user.UserId).ToList();
    }

    public async Task Add(InvoiceModel invoice)
    {
        context.Invoices.Add(invoice);

        await context.SaveChangesAsync();
        logger.LogInformation("Invoice added.");
    }

    public async Task Edit(InvoiceModel invoice)
    {
        var invoiceToUpdate = context.Invoices.FirstOrDefault(c => c.Id == invoice.Id);

        if (invoiceToUpdate != null)
        {
            invoiceToUpdate.XORs = invoice.XORs;

            context.Invoices.Update(invoiceToUpdate);

            await context.SaveChangesAsync();
            logger.LogInformation("Invoice updated.");
        }
        else
        {
            throw new ArgumentException("Invoice not found");
        }
    }

    public async Task Delete(InvoiceModel invoice)
    {
        var invoiceToDelete = context.Invoices.FirstOrDefault(c => c.Id == invoice.Id);

        if (invoiceToDelete != null)
        {
            context.Invoices.Remove(invoiceToDelete);

            await context.SaveChangesAsync();
            logger.LogInformation("Invoice removed.");
        }
        else
        {
            throw new ArgumentException("Invoice not found");
        }
    }

    public async Task DeleteAllOfUser(UserModel user)
    {
        var invoicesOfUser = GetAllOfUser(user);
        context.Invoices.RemoveRange(invoicesOfUser);

        await context.SaveChangesAsync();
    }
}