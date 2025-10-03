using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using xorWallet.Models;
using xorWallet.Services.Interfaces;

namespace xorWallet.Services;

public class InvoiceService(DatabaseContext context, ILogger<InvoiceService> logger) : IInvoiceService
{
    public IEnumerable<InvoiceModel> GetAllInvoices()
    {
        return context.Invoices.OrderByDescending(c => c.Id).Take(20).AsNoTracking().AsEnumerable();
    }

    public InvoiceModel? GetInvoiceById(ObjectId id)
    {
        return context.Invoices.FirstOrDefault(c => c.Id == id);
    }

    public InvoiceModel? GetInvoiceById(string id)
    {
        return context.Invoices.FirstOrDefault(c => c.InvoiceId == id);
    }

    public List<InvoiceModel> GetInvoicesOfUser(UserModel user)
    {
        return context.Invoices.Where(i => i.OwnerUserId == user.UserId).ToList();
    }

    public async Task AddInvoice(InvoiceModel invoice)
    {
        context.Invoices.Add(invoice);

        context.ChangeTracker.DetectChanges();
        logger.LogDebug("{ChangeTracker.DebugView.LongView}", context.ChangeTracker.DebugView.LongView);

        await context.SaveChangesAsync();
        logger.LogInformation("Invoice added.");
    }

    public async Task EditInvoice(InvoiceModel invoice)
    {
        var invoiceToUpdate = context.Invoices.FirstOrDefault(c => c.Id == invoice.Id);

        if (invoiceToUpdate != null)
        {
            invoiceToUpdate.XORs = invoice.XORs;

            context.Invoices.Update(invoiceToUpdate);

            context.ChangeTracker.DetectChanges();
            logger.LogDebug("{ChangeTracker.DebugView.LongView}", context.ChangeTracker.DebugView.LongView);

            await context.SaveChangesAsync();
            logger.LogInformation("Invoice updated.");
        }
        else
        {
            throw new ArgumentException("Invoice not found");
        }
    }

    public async Task DeleteInvoice(InvoiceModel invoice)
    {
        var invoiceToDelete = context.Invoices.FirstOrDefault(c => c.Id == invoice.Id);

        if (invoiceToDelete != null)
        {
            context.Invoices.Remove(invoiceToDelete);

            context.ChangeTracker.DetectChanges();
            logger.LogDebug("{ChangeTracker.DebugView}", context.ChangeTracker.DebugView.LongView);

            await context.SaveChangesAsync();
            logger.LogInformation("Invoice removed.");
        }
        else
        {
            throw new ArgumentException("Invoice not found");
        }
    }
}